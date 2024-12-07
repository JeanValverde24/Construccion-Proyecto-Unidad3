import os
from flask import Flask, json, request, jsonify, send_from_directory
import pymysql.cursors
import pandas as pd
import random
from sklearn.preprocessing import LabelEncoder
from sklearn.ensemble import RandomForestRegressor
app = Flask(__name__)
from flask_cors import CORS
CORS(app)

# Configuración de conexión con MySQL
def get_db_connection():
    connection = pymysql.connect(
        host='netdreams.pe',
        user='netdrepe_anthony',
        password='itIsnt4u',
        database='netdrepe_tesis',
        cursorclass=pymysql.cursors.DictCursor,
        ssl_disabled=True  # Desactiva SSL
    )
    return connection

# Ruta para login
@app.route('/api/login_turista', methods=['POST'])
def login_turista():
    data = request.get_json()
    correo = data.get("correo")
    password = data.get("password")
    connection = get_db_connection()

    with connection.cursor() as cursor:
        cursor.execute("""
            SELECT UsrId AS id, UsrTipoUsuario AS tipo_usuario, UsrNombresCompleto AS nombre 
            FROM TbUsuario 
            WHERE UsrCorreo = %s AND UsrContraseña = %s
        """, (correo, password))
        
        user = cursor.fetchone()

    if user:
        return jsonify({
            "status": "success",
            "id": user["id"],
            "tipo_usuario": user["tipo_usuario"],
            "nombre": user["nombre"]
        })
    else:
        return jsonify({
            "status": "error",
            "message": "Correo o contraseña incorrecta"
        }), 401


@app.route('/api/login_dueño', methods=['POST'])
def login_dueño():
    data = request.get_json()
    ruc = data.get("ruc")
    password = data.get("password")
    connection = get_db_connection()

    with connection.cursor() as cursor:
        cursor.execute("""
            SELECT UsrId AS id, UsrTipoUsuario AS tipo_usuario, UsrNombresCompleto AS nombre 
            FROM TbUsuario 
            WHERE UsrRuc = %s AND UsrContraseña = %s
        """, (ruc, password))
        
        user = cursor.fetchone()

    if user:
        return jsonify({
            "status": "success",
            "id": user["id"],
            "tipo_usuario": user["tipo_usuario"],
            "nombre": user["nombre"]
        })
    else:
        return jsonify({
            "status": "error",
            "message": "Ruc o contraseña incorrecta"
        }), 401

# Ruta para obtener negocios de un usuario
@app.route('/api/negocios', methods=['GET'])
def obtener_negocios():
    user_id = request.args.get('user_id')
    connection = get_db_connection()
    with connection.cursor() as cursor:
        cursor.execute("""
            SELECT 
                TbNegocio.TbNgcId AS negocio_id,
                TbNegocio.TbNgcNombre AS negocio_nombre,
                TbProvincia.TbPvncDescripcion AS provincia,
                TbTipoNegocio.TpNgcDescripcion AS tipo_negocio,
                TbNegocio.TbNgcDireccion AS direccion,  -- Incluye dirección
                TbNegocio.TbNgcTelefono AS telefono    -- Incluye teléfono
            FROM 
                TbNegocio
            LEFT JOIN 
                TbProvincia ON TbNegocio.TbNgcProvincia = TbProvincia.TbPvncId
            LEFT JOIN 
                TbTipoNegocio ON TbNegocio.TbNgcTipoNegocio = TbTipoNegocio.TpNgcId
            WHERE 
                TbNegocio.TbNgcUsuario = %s
        """, (user_id,))
        
        negocios = cursor.fetchall()
    
    if negocios:
        print("negocios: ", negocios)
        return jsonify(negocios)
    else:
        return jsonify({"message": "No se encontraron negocios para el usuario."}), 404


@app.route('/api/estadisticas/<int:negocio_id>', methods=['GET'])
def obtener_estadisticas(negocio_id):
    print("negocio estadistica:", negocio_id)
    try:
        connection = get_db_connection()
        with connection.cursor() as cursor:
            # Estadísticas de recomendaciones
            query_recomendaciones = """
                SELECT TipoServicio, COUNT(*) AS Cantidad
                FROM EstadisticasRecomendacion
                WHERE NegocioId = %s
                GROUP BY TipoServicio
            """
            cursor.execute(query_recomendaciones, (negocio_id,))
            estadisticas_recomendaciones = cursor.fetchall()

            # Estadísticas de visitas
            query_visitas = """
                SELECT Visitas
                FROM EstadisticasVisita
                WHERE IdNegocio = %s
            """
            cursor.execute(query_visitas, (negocio_id,))
            estadisticas_visitas = cursor.fetchone()

        return jsonify({
            "recomendaciones": estadisticas_recomendaciones,
            "visitas": estadisticas_visitas.get("Visitas", 0) if estadisticas_visitas else 0
        })
    except Exception as e:
        return jsonify({"error": str(e)}), 500
    finally:
        connection.close()
        

@app.route('/api/obtener_negocio/<int:negocio_id>', methods=['GET'])
def obtener_negocio(negocio_id):
    connection = get_db_connection()
    
    with connection.cursor() as cursor:
        # Consulta mejorada para incluir imágenes y el nombre de la provincia
        cursor.execute("""
            SELECT 
                TbNegocio.TbNgcId AS negocio_id,
                TbNegocio.TbNgcNombre AS negocio_nombre,
                TbProvincia.TbPvncDescripcion AS provincia_nombre, -- Nombre de la provincia
                TbTipoNegocio.TpNgcDescripcion AS tipo_negocio,
                TbNegocio.TbNgcDireccion AS direccion,       -- Dirección
                TbNegocio.TbNgcTelefono AS telefono,        -- Teléfono
                GROUP_CONCAT(TbImagenNegocio.TbImgRuta) AS imagenes -- Imágenes del negocio
            FROM 
                TbNegocio
            LEFT JOIN 
                TbProvincia ON TbNegocio.TbNgcProvincia = TbProvincia.TbPvncId
            LEFT JOIN 
                TbTipoNegocio ON TbNegocio.TbNgcTipoNegocio = TbTipoNegocio.TpNgcId
            LEFT JOIN 
                TbImagenNegocio ON TbNegocio.TbNgcId = TbImagenNegocio.TbImgNegocioId -- Asociación con imágenes
            WHERE 
                TbNegocio.TbNgcId = %s
            GROUP BY 
                TbNegocio.TbNgcId, TbProvincia.TbPvncDescripcion, TbTipoNegocio.TpNgcDescripcion
        """, (negocio_id,))
        
        negocio = cursor.fetchone()
        
    if negocio:
        # Formatear las imágenes como una lista
        imagenes_list = negocio['imagenes'].split(',') if negocio['imagenes'] else []
                # Construir la respuesta
        negocio_dict = {
            'negocio_id': negocio['negocio_id'],
            'TbNgcNombre': negocio['negocio_nombre'],
            'TipoNegocio': negocio['tipo_negocio'],
            'Provincia': negocio['provincia_nombre'],  # Devuelve el nombre
            'TbNgcDireccion': negocio['direccion'],         # Devuelve dirección
            'TbNgcTelefono': negocio['telefono'],           # Devuelve teléfono
            'Imagenes': imagenes_list                       # Lista de imágenes
        }
        print(negocio_dict)
        return jsonify(negocio_dict)
    else:
        return jsonify({"message": "Negocio no encontrado"}), 404



@app.route('/api/hoteles/agregar_cuarto', methods=['POST'])
def agregar_servicio_hotel():
    data = request.get_json()
    
    # Log para verificar los datos recibidos
    print("Datos recibidos en la API:", data)

    # Ajuste: Cambiar a "negocioId" para coincidir con el JSON enviado desde ASP.NET
    negocio_id = data.get('negocioId')
    cantidad_personas = data.get('cantidadPersonas')
    wifi = data.get('wifi')
    agua_caliente = data.get('aguaCaliente')
    room_service = data.get('roomService')
    cochera = data.get('cochera')
    cable = data.get('cable')
    desayuno_incluido = data.get('desayunoIncluido')
    precio = data.get('precio')
    fotos = data.get('fotos')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = """
                INSERT INTO ServicioHotel 
                (NegocioId, CantidadPersonas, WiFi, AguaCaliente, RoomService, Cochera, Cable, DesayunoIncluido, Precio, Fotos) 
                VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
            """
            cursor.execute(query, (negocio_id, cantidad_personas, wifi, agua_caliente, room_service, cochera, cable, desayuno_incluido, precio, fotos))
            connection.commit()
        return jsonify({"message": "Servicio de hotel agregado con éxito"}), 201
    except Exception as e:
        return jsonify({"error": str(e)}), 500




@app.route('/api/restaurantes/agregar_plato', methods=['POST'])
def agregar_servicio_restaurante():
    data = request.get_json()
    negocio_id = data.get('negocioId')
    nombre_plato = data.get('nombrePlato')
    tipo_plato = data.get('tipoPlato')
    precio = data.get('precio')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = """
                INSERT INTO ServicioRestaurante 
                (NegocioId, NombrePlato, TipoPlato, Precio) 
                VALUES (%s, %s, %s, %s)
            """
            cursor.execute(query, (negocio_id, nombre_plato, tipo_plato, precio))
            connection.commit()
        return jsonify({"message": "Plato agregado con éxito"}), 201
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/api/turismo/agregar_lugar', methods=['POST'])
def agregar_servicio_turistico():
    data = request.get_json()
    negocio_id = data.get('negocioId')
    nombre_lugar = data.get('nombreLugar')
    descripcion = data.get('descripcion')
    precio = data.get('precio')
    provincia = data.get('provincia')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = """
                INSERT INTO ServicioTuristico 
                (NegocioId, NombreLugar, Descripcion, Precio, Provincia) 
                VALUES (%s, %s, %s, %s, %s)
            """
            cursor.execute(query, (negocio_id, nombre_lugar, descripcion, precio, provincia))
            connection.commit()
        return jsonify({"message": "Lugar turístico agregado con éxito"}), 201
    except Exception as e:
        return jsonify({"error": str(e)}), 500




# Ruta para obtener servicios de hotel según el negocioId
@app.route('/api/hoteles/obtener_servicios', methods=['GET'])
def obtener_servicios_hotel():
    negocio_id = request.args.get('negocioId')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = "SELECT * FROM ServicioHotel WHERE NegocioId = %s"
            cursor.execute(query, (negocio_id,))
            servicios = cursor.fetchall()
        return jsonify(servicios), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500


# Ruta para obtener servicios de restaurante según el negocioId
@app.route('/api/restaurantes/obtener_servicios', methods=['GET'])
def obtener_servicios_restaurante():
    negocio_id = request.args.get('negocioId')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = "SELECT * FROM ServicioRestaurante WHERE NegocioId = %s"
            cursor.execute(query, (negocio_id,))
            servicios = cursor.fetchall()
        return jsonify(servicios), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500

# Ruta para obtener servicios de turismo según el negocioId
@app.route('/api/turismo/obtener_servicios', methods=['GET'])
def obtener_servicios_turismo():
    negocio_id = request.args.get('negocioId')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = "SELECT * FROM ServicioTuristico WHERE NegocioId = %s"
            cursor.execute(query, (negocio_id,))
            servicios = cursor.fetchall()
        return jsonify(servicios), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500
    


# Ruta para actualizar el servicio de hotel
import traceback

@app.route('/api/hotel/actualizar_servicio', methods=['PUT'])
def actualizar_servicio_hotel():
    data = request.get_json()
    
    # Imprimir los datos recibidos para verificar su contenido
    print("Datos recibidos:", data)

    if not data or 'NegocioId' not in data:
        return jsonify({"error": "No se recibieron datos o falta 'NegocioId'"}), 400

    negocio_id = data['Id']
    cantidad_personas = data.get('CantidadPersonas')
    wifi = data.get('Wifi')
    agua_caliente = data.get('AguaCaliente')
    room_service = data.get('RoomService')
    cochera = data.get('Cochera')
    cable = data.get('Cable')
    desayuno_incluido = data.get('DesayunoIncluido')
    precio = data.get('Precio')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = """
                UPDATE ServicioHotel SET 
                    CantidadPersonas = %s, WiFi = %s, AguaCaliente = %s, 
                    RoomService = %s, Cochera = %s, Cable = %s, 
                    DesayunoIncluido = %s, Precio = %s
                WHERE Id = %s
            """
            cursor.execute(query, (cantidad_personas, wifi, agua_caliente, 
                                   room_service, cochera, cable, 
                                   desayuno_incluido, precio, negocio_id))
            connection.commit()
        
        return jsonify({"message": "Servicio de hotel actualizado con éxito"}), 200

    except Exception as e:
        print("Error en la actualización del servicio de hotel:", str(e))
        return jsonify({"error": str(e)}), 500


# Ruta para actualizar el servicio de restaurante
@app.route('/api/restaurante/actualizar_servicio', methods=['PUT'])
def actualizar_servicio_restaurante():
    data = request.get_json()
    print("Datos recibidos:", data)

    if not data or 'NegocioId' not in data:
        return jsonify({"error": "No se recibieron datos o falta 'NegocioId'"}), 400

    negocio_id = data['NegocioId']
    nombre_plato = data.get('NombrePlato')
    tipo_comida = data.get('TipoPlato')
    precio = data.get('Precio')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = """
                UPDATE ServicioRestaurante SET 
                    NombrePlato = %s, TipoPlato = %s, Precio = %s
                WHERE NegocioId = %s
            """
            cursor.execute(query, (nombre_plato, tipo_comida, precio, negocio_id))
            connection.commit()
        
        return jsonify({"message": "Servicio de restaurante actualizado con éxito"}), 200

    except Exception as e:
        return jsonify({"error": str(e)}), 500

# Ruta para actualizar el servicio de turismo
@app.route('/api/turismo/actualizar_servicio', methods=['PUT'])
def actualizar_servicio_turismo():
    data = request.get_json()
    
    if not data or 'NegocioId' not in data:
        return jsonify({"error": "No se recibieron datos o falta 'NegocioId'"}), 400

    negocio_id = data['NegocioId']
    nombre_lugar = data.get('NombreLugar')
    descripcion = data.get('Descripcion')
    duracion = data.get('Duracion')
    precio = data.get('Precio')
    provincia = data.get('Provincia')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            query = """
                UPDATE ServicioTuristico SET 
                    NombreLugar = %s, Descripcion = %s, Precio = %s, Provincia = %s
                WHERE NegocioId = %s
            """
            cursor.execute(query, (nombre_lugar, descripcion, precio, provincia, negocio_id))
            connection.commit()
        
        return jsonify({"message": "Servicio de turismo actualizado con éxito"}), 200

    except Exception as e:
        return jsonify({"error": str(e)}), 500

from flask import Flask, request, jsonify
from sqlalchemy import create_engine
import pandas as pd
from sklearn.ensemble import RandomForestRegressor
from sklearn.preprocessing import LabelEncoder
from datetime import datetime
import pymysql
from sqlalchemy import text
DB_CONFIG = {
    "user": "netdrepe_anthony",  # Cambia por tu usuario
    "password": "itIsnt4u",  # Cambia por tu contraseña
    "host": "netdreams.pe",  # Cambia por tu host
    "database": "netdrepe_tesis",  # Cambia por el nombre de tu base de datos
}

# Crear la URL de conexión con SQLAlchemy
DATABASE_URL = f"mysql+pymysql://{DB_CONFIG['user']}:{DB_CONFIG['password']}@{DB_CONFIG['host']}/{DB_CONFIG['database']}"
engine = create_engine(DATABASE_URL)

# Función para cargar los datos desde la base de datos
def cargar_datos():
    try:
        query_hoteles = """
            SELECT sh.Precio AS 'Precio por Noche', sh.CantidadPersonas AS 'Capacidad Habitación',
                   (CASE 
                    WHEN sh.Wifi = 'si' THEN 1 ELSE 0 END) + 
                   (CASE 
                    WHEN sh.AguaCaliente = 'si' THEN 1 ELSE 0 END) + 
                   (CASE 
                    WHEN sh.RoomService = 'si' THEN 1 ELSE 0 END) + 
                   (CASE 
                    WHEN sh.Cochera = 'si' THEN 1 ELSE 0 END) + 
                   (CASE 
                    WHEN sh.Cable = 'si' THEN 1 ELSE 0 END) + 
                   (CASE 
                    WHEN sh.DesayunoIncluido = 'si' THEN 1 ELSE 0 END) AS 'Servicios',
                   tn.TbNgcNombre AS 'Nombre del Hotel', tn.TbNgcId AS 'NegocioId'
            FROM ServicioHotel sh
            JOIN TbNegocio tn ON tn.TbNgcId = sh.NegocioId
            WHERE tn.TbNgcTipoNegocio = 1"""  # Consulta SQL para hoteles
        query_restaurantes = """ SELECT tn.TbNgcNombre AS 'Nombre del Restaurante', 
                   AVG(sr.Precio) AS 'Precio Medio por Plato', 
                   tn.TbNgcId AS 'NegocioId'
            FROM ServicioRestaurante sr
            JOIN TbNegocio tn ON tn.TbNgcId = sr.NegocioId
            WHERE tn.TbNgcTipoNegocio = 2
            GROUP BY tn.TbNgcId"""  # Consulta SQL para restaurantes
        query_lugares = """   SELECT tn.TbNgcNombre AS 'Nombre del Lugar', st.Precio AS 'Costo Entrada', tn.TbNgcId AS 'NegocioId'
            FROM ServicioTuristico st
            JOIN TbNegocio tn ON tn.TbNgcId = st.NegocioId
            WHERE tn.TbNgcTipoNegocio = 3"""  # Consulta SQL para lugares turísticos

        df_hoteles = pd.read_sql(query_hoteles, engine)
        df_restaurantes = pd.read_sql(query_restaurantes, engine)
        df_lugares = pd.read_sql(query_lugares, engine)

        return df_hoteles, df_restaurantes, df_lugares
    except Exception as e:
        print(f"Error al cargar los datos: {e}")
        return None, None, None

# Función para verificar fechas especiales
fechas_especiales = [
    {"inicio": "2024-12-20", "fin": "2024-12-31", "ajuste": 1.2},
    {"inicio": "2024-01-01", "fin": "2024-01-07", "ajuste": 0.9}
]
def guardar_estadisticas(negocio_id, servicio_id, tipo_servicio):
    # Asigna un valor predeterminado si el servicio_id es None
    if servicio_id is None:
        servicio_id = 0  # O el valor que prefieras como predeterminado
        print(f"ServicioId era None, asignando valor predeterminado: {servicio_id}")
    
    print(f"Datos a insertar: {negocio_id}, {servicio_id}, {tipo_servicio}")
    try:
        query = """
            INSERT INTO EstadisticasRecomendacion (NegocioId, ServicioId, TipoServicio)
            VALUES (:negocio_id, :servicio_id, :tipo_servicio)
        """
        params = {
            'negocio_id': negocio_id,
            'servicio_id': servicio_id,
            'tipo_servicio': tipo_servicio
        }

        with engine.connect() as connection:
            connection.execute(text(query), params)  # Pasa los parámetros como un diccionario
            connection.commit()  # Confirma los cambios
            print(f"Estadística registrada: NegocioId={negocio_id}, ServicioId={servicio_id}, TipoServicio={tipo_servicio}")
    except Exception as e:
        print(f"Error al guardar estadísticas: {e}")

def verificar_fechas_especiales(fecha_inicio, fecha_fin):
    fecha_inicio = datetime.strptime(fecha_inicio, "%Y-%m-%d")
    fecha_fin = datetime.strptime(fecha_fin, "%Y-%m-%d")
    for rango in fechas_especiales:
        inicio_especial = datetime.strptime(rango["inicio"], "%Y-%m-%d")
        fin_especial = datetime.strptime(rango["fin"], "%Y-%m-%d")
        if inicio_especial <= fecha_fin and fin_especial >= fecha_inicio:
            return rango["ajuste"]  # Devuelve directamente el factor de ajuste
    return 1.0  # Si no hay fechas especiales, retorna 1.0 (sin ajuste)

# Función para generar cotizaciones
def generar_cotizaciones(ubicacion_usuario, presupuesto_max, dias_viaje, cantidad_personas, fecha_inicio, fecha_fin):
    df_hoteles, df_restaurantes, df_lugares = cargar_datos()
    if df_hoteles is None or df_restaurantes is None or df_lugares is None:
        return {"error": "No se pudieron cargar los datos"}
    
    # Verificar fechas especiales
    factor_ajuste = verificar_fechas_especiales(fecha_inicio, fecha_fin)
    resultados = []

    # Filtros y lógica
    hoteles_ajustados = df_hoteles[df_hoteles['Precio por Noche'] * dias_viaje <= presupuesto_max]
    restaurantes_ajustados = df_restaurantes[df_restaurantes['Precio Medio por Plato'] * cantidad_personas <= presupuesto_max]
    lugares_ajustados = df_lugares[df_lugares['Costo Entrada'] * cantidad_personas <= presupuesto_max]

    if hoteles_ajustados.empty or restaurantes_ajustados.empty or lugares_ajustados.empty:
        return {"error": "No se encontraron opciones dentro del presupuesto"}

    porcentajes = [0.8, 0.9, 0.95, 1.0, 1.1]  # Porcentajes requeridos
    for porcentaje in porcentajes:
        presupuesto_ajustado = presupuesto_max * porcentaje
        for _, hotel in hoteles_ajustados.iterrows():
            for _, restaurante in restaurantes_ajustados.iterrows():
                for _, lugar in lugares_ajustados.iterrows():
                    # Calcular los costos
                    costo_hotel = round(hotel['Precio por Noche'] * dias_viaje, 2)
                    costo_restaurante = round(restaurante['Precio Medio por Plato'] * cantidad_personas, 2)
                    costo_lugar = round(lugar['Costo Entrada'] * cantidad_personas, 2)

                    # Aplicar ajuste si corresponde
                    if factor_ajuste != 1.0:
                        costo_hotel = round(costo_hotel * factor_ajuste, 2)
                        costo_restaurante = round(costo_restaurante * factor_ajuste, 2)
                        costo_lugar = round(costo_lugar * factor_ajuste, 2)

                    # Calcular el total
                    costo_total = round(costo_hotel + costo_restaurante + costo_lugar, 2)
                    presupuesto_restante = round(presupuesto_ajustado - costo_total, 2)

                    if presupuesto_restante >= 0:
                        # Guardar estadísticas
                        guardar_estadisticas(hotel['NegocioId'], hotel['NegocioId'], 'hotel')
                        guardar_estadisticas(restaurante['NegocioId'], None, 'restaurante')
                        guardar_estadisticas(lugar['NegocioId'], lugar['NegocioId'], 'lugar')

                        # Agregar resultados
                        resultados.append({
                            "Hotel": hotel['Nombre del Hotel'],
                            "CostoHotel": costo_hotel,
                            "Restaurante": restaurante['Nombre del Restaurante'],
                            "CostoRestaurante": costo_restaurante,
                            "Lugar": lugar['Nombre del Lugar'],
                            "CostoLugar": costo_lugar,
                            "Total": costo_total,
                            "PresupuestoRestante": presupuesto_restante,
                            "PorcentajePresupuesto": round(porcentaje * 100, 2),
                            "AjusteEspecial": factor_ajuste
                        })
                        break
                if resultados:
                    break
            if resultados:
                break

    return resultados

# Endpoint para la API
@app.route('/api/generar_cotizaciones', methods=['POST'])
def api_generar_cotizaciones():
    datos = request.json
    try:
        ubicacion_usuario = datos['ubicacion_usuario']
        presupuesto_max = float(datos['presupuesto_max'])
        dias_viaje = int(datos['dias_viaje'])
        cantidad_personas = int(datos['cantidad_personas'])
        fecha_inicio = datos['fecha_inicio']
        fecha_fin = datos['fecha_fin']

        resultados = generar_cotizaciones(
            ubicacion_usuario,
            presupuesto_max,
            dias_viaje,
            cantidad_personas,
            fecha_inicio,
            fecha_fin
        )
        print ("cotizaciones: ", resultados)
        return jsonify(resultados)
    except Exception as e:
        return jsonify({"error": str(e)}), 400
    

import numpy as np
# Definir la ruta de la API
def convertir_a_tipos_nativos(data):
    if isinstance(data, dict):
        return {k: convertir_a_tipos_nativos(v) for k, v in data.items()}
    elif isinstance(data, list):
        return [convertir_a_tipos_nativos(i) for i in data]
    elif isinstance(data, np.integer):  # Convierte numpy.int64 a int
        return int(data)
    elif isinstance(data, np.floating):  # Convierte numpy.float64 a float
        return float(data)
    else:
        return data

@app.route('/api/registro/turista', methods=['POST'])
def registrar_turista():
    data = request.get_json()
    print("[DEBUG] Datos recibidos para registrar turista:", data)

    # Validación de datos requeridos
    required_fields = ["UsrNombresCompleto", "UsrCorreo", "UsrContraseña", "UsrDniRut"]
    for field in required_fields:
        if field not in data or not data[field]:
            print(f"[DEBUG] Validación fallida: {field} es requerido")
            return jsonify({"error": f"{field} es requerido"}), 400

    # Verificar unicidad del DNI o RUT
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            sql_check = "SELECT COUNT(*) AS count FROM TbUsuario WHERE UsrDniRut = %s"
            cursor.execute(sql_check, (data["UsrDniRut"],))
            result = cursor.fetchone()
            print(f"[DEBUG] Resultado de la verificación de unicidad: {result}")
            if result and result["count"] > 0:
                return jsonify({"error": "El DNI o RUT ya está registrado"}), 409

            # Insertar el nuevo usuario
            sql = """
                INSERT INTO TbUsuario (UsrDniRut, UsrNombresCompleto, 
                                       UsrCorreo, UsrTipoUsuario, UsrEstado, UsrContraseña)
                VALUES (%s, %s, %s, %s, %s, %s)
            """
            tipo_usuario = 1  # Turista
            estado = 1  # Activo
            cursor.execute(sql, (
                data["UsrDniRut"],
                data["UsrNombresCompleto"],
                data["UsrCorreo"],
                tipo_usuario,
                estado,
                data["UsrContraseña"]
            ))
            connection.commit()
            # Capturamos el ID del usuario recién insertado
            turista_id = cursor.lastrowid
            print("[DEBUG] Usuario turista registrado exitosamente, ID:", turista_id)

        return jsonify({"message": "Turista registrado exitosamente", "id": turista_id}), 201

    except pymysql.MySQLError as e:
        print("[ERROR] Error de MySQL:", str(e))
        return jsonify({"error": str(e)}), 500

    finally:
        print("[DEBUG] Cerrando conexión con la base de datos")
        connection.close()



@app.route('/api/registro/socio', methods=['POST'])
def registrar_socio():
    data = request.get_json()
    print("[DEBUG] Datos recibidos para registrar socio:", data)

    # Validación de datos requeridos
    required_fields = ["UsrNombresCompleto", "UsrCorreo", "UsrRuc", "UsrContraseña"]
    for field in required_fields:
        if field not in data or not data[field]:
            print(f"[DEBUG] Validación fallida: {field} es requerido")
            return jsonify({"error": f"{field} es requerido"}), 400

    # Verificar unicidad del RUC
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            sql_check = "SELECT COUNT(*) FROM TbUsuario WHERE UsrRuc = %s"
            print("[DEBUG] Ejecutando consulta de unicidad:", sql_check)
            cursor.execute(sql_check, (data["UsrRuc"],))
            result = cursor.fetchone()
            print("[DEBUG] Resultado de la verificación de unicidad:", result)

            if result and (result[0] > 0 if isinstance(result, tuple) else result["COUNT(*)"] > 0):
                print("[DEBUG] El RUC ya está registrado")
                return jsonify({"error": "El RUC ya está registrado"}), 409

            sql = """
                INSERT INTO TbUsuario (UsrRuc, UsrNombresCompleto, UsrCorreo, UsrTipoUsuario, UsrEstado, UsrContraseña)
                VALUES (%s, %s, %s, %s, %s, %s)
            """
            print("[DEBUG] Ejecutando consulta de inserción:", sql)
            tipo_usuario = 2  # Socio
            estado = 1  # Activo
            cursor.execute(sql, (
                data["UsrRuc"],
                data["UsrNombresCompleto"],
                data["UsrCorreo"],
                tipo_usuario,
                estado,
                data["UsrContraseña"]
            ))
            connection.commit()
            socio_id = cursor.lastrowid
            print("[DEBUG] Socio registrado exitosamente, ID:", socio_id)

        return jsonify({"message": "Socio registrado exitosamente", "id": socio_id}), 201

    except pymysql.MySQLError as e:
        print("[ERROR] Error de MySQL:", str(e))
        return jsonify({"error": str(e)}), 500

    finally:
        print("[DEBUG] Cerrando conexión con la base de datos")
        connection.close()


# Endpoint para obtener los tipos de negocio
@app.route('/api/tipos_negocio', methods=['GET'])
def obtener_tipos_negocio():
    connection = get_db_connection()
    with connection.cursor() as cursor:
        cursor.execute("SELECT TpNgcId, TpNgcDescripcion AS Nombre FROM TbTipoNegocio")
        tipos_negocio = cursor.fetchall()
    connection.close()
    return jsonify(tipos_negocio)


@app.route('/api/actualizarnegocio/<int:negocio_id>', methods=['PUT'])
def actualizar_negocio(negocio_id):
    negocio_data = request.get_json()

    # Logs para depuración
    print("Payload recibido:", negocio_data)

    nombre_negocio = negocio_data.get('TbNgcNombre')
    tipo_negocio = negocio_data.get('TipoNegocio')
    provincia_id = negocio_data.get('ProvinciaId')
    direccion = negocio_data.get('TbNgcDireccion')  # Nuevo campo
    telefono = negocio_data.get('TbNgcTelefono')    # Nuevo campo

    connection = get_db_connection()
    with connection.cursor() as cursor:
        cursor.execute("""
            UPDATE TbNegocio
            SET 
                TbNgcNombre = %s, 
                TbNgcProvincia = %s,
                TbNgcDireccion = %s,
                TbNgcTelefono = %s
            WHERE TbNgcId = %s
        """, (nombre_negocio, provincia_id, direccion, telefono, negocio_id))
        connection.commit()

    return jsonify({"message": "Negocio actualizado correctamente"}), 200


# Endpoint para obtener las provincias
@app.route('/api/provincias', methods=['GET'])

def obtener_provincias():
    connection = get_db_connection()
    with connection.cursor() as cursor:
        # Ejecuta una consulta básica para verificar la conexión
        cursor.execute("SELECT 1")
        # Luego ejecuta la consulta real
        cursor.execute("SELECT TbPvncId, TbPvncDescripcion AS Nombre FROM TbProvincia")
        provincias = cursor.fetchall()
    connection.close()
    return jsonify(provincias)
    
# Configuración de la carpeta de subida
UPLOAD_FOLDER = 'uploads/negocios'  # Define la carpeta donde se guardarán las imágenes
app.config['UPLOAD_FOLDER'] = UPLOAD_FOLDER
os.makedirs(UPLOAD_FOLDER, exist_ok=True)  # Crear carpeta si no existe
from werkzeug.utils import secure_filename


@app.route('/api/registro/negocio', methods=['POST'])
def registrar_negocio():
    try:
        # Obtener datos del formulario
        negocio_data = json.loads(request.form.get("negocio"))
        print("Datos del negocio recibidos:", negocio_data)

        connection = get_db_connection()
        with connection.cursor() as cursor:
            # Insertar datos del negocio
            sql_negocio = """
                INSERT INTO TbNegocio (TbNgcUsuario, TbNgcNombre, TbNgcTipoNegocio, TbNgcProvincia, TbNgcDireccion, TbNgcTelefono)
                VALUES (%s, %s, %s, %s, %s, %s)
            """
            cursor.execute(sql_negocio, (
                negocio_data["TbNgcUsuario"],
                negocio_data["TbNgcNombre"],
                negocio_data["TbNgcTipoNegocio"],
                negocio_data["TbNgcProvincia"],
                negocio_data["TbNgcDireccion"],
                negocio_data["TbNgcTelefono"]
            ))
            negocio_id = cursor.lastrowid

            # Procesar y guardar imágenes si existen
            for img_file in request.files.getlist("TbImgRuta"):
                if img_file:
                    filename = secure_filename(img_file.filename)
                    img_path = os.path.join(app.config['UPLOAD_FOLDER'], filename)
                    img_file.save(img_path)

                    sql_imagen = """
                        INSERT INTO TbImagenNegocio (TbImgNegocioId, TbImgRuta)
                        VALUES (%s, %s)
                    """
                    cursor.execute(sql_imagen, (negocio_id, img_path))

            connection.commit()
            return jsonify({"message": "Negocio registrado exitosamente"}), 201

    except Exception as e:
        print("Error en el registro del negocio:", e)
        return jsonify({"error": "Error en el registro del negocio"}), 500

    finally:
        if 'connection' in locals():
            connection.close()


@app.route('/api/listanegocios', methods=['GET'])
def negocios():
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            sql = """
                SELECT 
                    n.TbNgcId,
                    n.TbNgcNombre,
                    n.TbNgcUsuario,
                    n.TbNgcTipoNegocio,
                    tn.TpNgcDescripcion AS TipoNegocio,
                    n.TbNgcProvincia,
                    p.TbPvncDescripcion AS Provincia,
                    i.TbImgRuta AS ImagenUrl
                FROM TbNegocio n
                LEFT JOIN TbProvincia p ON n.TbNgcProvincia = p.TbPvncId
                LEFT JOIN TbTipoNegocio tn ON n.TbNgcTipoNegocio = tn.TpNgcId
                LEFT JOIN TbImagenNegocio i ON n.TbNgcId = i.TbImgNegocioId
            """
            cursor.execute(sql)
            rows = cursor.fetchall()

            # Agrupar imágenes por negocio
            negocios = {}
            for row in rows:
                negocio_id = row['TbNgcId']
                if negocio_id not in negocios:
                    negocios[negocio_id] = {
                        "negocio_id": row["TbNgcId"],
                        "TbNgcNombre": row["TbNgcNombre"],
                        "TbNgcUsuario": row["TbNgcUsuario"],
                        "TbNgcTipoNegocio": row["TbNgcTipoNegocio"],
                        "TipoNegocio": row["TipoNegocio"],
                        "TbNgcProvincia": row["TbNgcProvincia"],
                        "Provincia": row["Provincia"],
                        "ImagenesUrl": []
                    }
                if row["ImagenUrl"]:
                    negocios[negocio_id]["ImagenesUrl"].append(row["ImagenUrl"])
            return jsonify(list(negocios.values())), 200
    except Exception as e:
        print("Error:", e)
        return jsonify({"error": "Error al obtener los negocios"}), 500
    finally:
        connection.close()


FOLDER = os.path.join(os.getcwd(), 'uploads')
@app.route('/uploads/negocios/<path:filename>')
def uploaded_file(filename):
    # Ajustar para no repetir 'negocios' en la ruta
    return send_from_directory(os.path.join(FOLDER, 'negocios'), filename)
from datetime import datetime

@app.route('/api/enviar_feedback', methods=['POST'])
def enviar_feedback():
    data = request.get_json()
    negocio_id = data.get('negocio_id')
    usuario_id = data.get('usuario_id')
    comentario = data.get('comentario')
    calificacion = data.get('calificacion')

    # Conectar con la base de datos
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            # Crear el query para insertar el feedback
            sql = """
                INSERT INTO TbFeedback (FbNegocioId, FbUsuarioId, FbComentario, FbCalificacion, FbFecha)
                VALUES (%s, %s, %s, %s, %s)
            """
            cursor.execute(sql, (negocio_id, usuario_id, comentario, calificacion, datetime.now()))
            connection.commit()

        return jsonify({"message": "Feedback guardado exitosamente"}), 201
    except Exception as e:
        print(f"Error al guardar el feedback: {e}")
        return jsonify({"message": "Error al guardar el feedback"}), 500
    finally:
        connection.close()


@app.route('/api/feedbacks/<int:user_id>', methods=['GET'])
def get_feedbacks(user_id):
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            sql = """
                SELECT TbFeedback.FbCalificacion, TbFeedback.FbComentario, TbFeedback.FbFecha, TbFeedback.FbId, 
                       TbFeedback.FbNegocioId, TbFeedback.FbUsuarioId,
                       TbNegocio.TbNgcNombre AS NegocioNombre
                FROM TbFeedback
                JOIN TbNegocio ON TbFeedback.FbNegocioId = TbNegocio.TbNgcId
                WHERE TbFeedback.FbUsuarioId = %s
            """
            cursor.execute(sql, (user_id,))
            feedbacks = cursor.fetchall()

        # Transformar los resultados en un formato JSON serializable
        feedbacks_data = [
            {
                "FbCalificacion": feedback["FbCalificacion"],
                "FbComentario": feedback["FbComentario"],
                "FbFecha": feedback["FbFecha"].strftime("%a, %d %b %Y %H:%M:%S GMT"),
                "FbId": feedback["FbId"],
                "FbNegocioId": feedback["FbNegocioId"],
                "FbUsuarioId": feedback["FbUsuarioId"],
                "NegocioNombre": feedback["NegocioNombre"],
                
            }
            for feedback in feedbacks
        ]
        
        return jsonify(feedbacks_data), 200
    finally:
        connection.close()


# Ruta para obtener negocios de un usuario con feedbacks
@app.route('/api/negocios_feedbacks', methods=['GET'])
def obtener_negocios_con_feedbacks():
    user_id = request.args.get('user_id')
    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            # Consulta para obtener los negocios del usuario
            cursor.execute("""
                SELECT 
                    TbNegocio.TbNgcId AS negocio_id,
                    TbNegocio.TbNgcNombre AS negocio_nombre,
                    TbProvincia.TbPvncDescripcion AS provincia,
                    TbTipoNegocio.TpNgcDescripcion AS tipo_negocio
                FROM 
                    TbNegocio
                LEFT JOIN 
                    TbProvincia ON TbNegocio.TbNgcProvincia = TbProvincia.TbPvncId
                LEFT JOIN 
                    TbTipoNegocio ON TbNegocio.TbNgcTipoNegocio = TbTipoNegocio.TpNgcId
                WHERE 
                    TbNegocio.TbNgcUsuario = %s
            """, (user_id,))
            negocios = cursor.fetchall()

            # Para cada negocio, obtener sus feedbacks
            for negocio in negocios:
                cursor.execute("""
                    SELECT 
                        FbCalificacion,
                        FbComentario,
                        FbFecha,
                        FbId,
                        FbUsuarioId
                    FROM 
                        TbFeedback
                    WHERE 
                        FbNegocioId = %s
                """, (negocio['negocio_id'],))
                feedbacks = cursor.fetchall()
                negocio['feedbacks'] = feedbacks

        return jsonify(negocios), 200
    finally:
        connection.close()
# Ruta para incrementar las visitas
@app.route('/api/incrementar_visitas', methods=['POST'])
def incrementar_visitas():
    data = request.get_json()
    negocio_id = data.get('negocioId')

    if not negocio_id:
        return jsonify({"error": "Falta el parámetro 'negocioId'"}), 400

    connection = get_db_connection()
    try:
        with connection.cursor() as cursor:
            # Incrementar visitas o crear un registro si no existe
            query = """
            INSERT INTO EstadisticasVisita (IdNegocio, Visitas)
            VALUES (%s, 1)
            ON DUPLICATE KEY UPDATE Visitas = Visitas + 1;
            """
            cursor.execute(query, (negocio_id,))
            connection.commit()

        return jsonify({"success": True, "message": f"Visitas incrementadas para el negocio con ID {negocio_id}"}), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500
    finally:
        connection.close()

if __name__ == '__main__':
    app.run(debug=True)
