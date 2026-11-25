Algoritmo MesaDePartesDigital_Prototipo
	Definir maxExp, totalReg, i, j, opcion, pos Como Entero
	Definir codIngresado, codLimpio, docIngresado, docLimpio, rem, asu, area, busq Como Cadena
	Definir tempCod, tempDoc, tempRem, tempAsu, tempArea Como Cadena
	Definir esValido Como Logico
	
	maxExp <- 100
	totalReg <- 0
	
	Dimension codigos[maxExp]
	Dimension documentos[maxExp]
	Dimension remitentes[maxExp]
	Dimension asuntos[maxExp]
	Dimension areas[maxExp]
	
	Repetir
		Limpiar Pantalla
		Escribir "=========================================="
		Escribir "   MESA DE PARTES DIGITAL (PROTOTIPO)   "
		Escribir "=========================================="
		Escribir "1. Registrar expediente"
		Escribir "2. Consultar expediente por codigo"
		Escribir "3. Ordenar expedientes (Burbuja)"
		Escribir "4. Listar expedientes"
		Escribir "5. Salir"
		Escribir "Seleccione una opcion (1-5): "
		Leer opcion
		
		Segun opcion Hacer
			1:
				Limpiar Pantalla
				Escribir "--- REGISTRO DE EXPEDIENTE ---"
				Si totalReg >= maxExp Entonces
					Escribir "Error: Capacidad maxima alcanzada."
				Sino
					Escribir "Ingrese codigo (Ej. EXP-001): "
					Leer codIngresado
					codLimpio <- Mayusculas(codIngresado)
					
					// Validacion sencilla de formato EXP-### (7 caracteres: EXP-001)
					esValido <- Verdadero
					Si Longitud(codLimpio) <> 7 O Subcadena(codLimpio, 1, 4) <> "EXP-" Entonces
						esValido <- Falso
					FinSi
					
					Si NO esValido Entonces
						Escribir "Error: Formato invalido. Use EXP-### (Ej. EXP-001)."
					Sino
						// Verificar si el codigo ya existe
						pos <- -1
						Para i <- 0 Hasta totalReg - 1 Hacer
							Si codigos[i] = codLimpio Entonces
								pos <- i
							FinSi
						FinPara
						
						Si pos <> -1 Entonces
							Escribir "Error: Ese codigo ya esta registrado."
						Sino
							Escribir "Ingrese DNI (8 digitos) o RUC (11 digitos): "
							Leer docIngresado
							docLimpio <- docIngresado
							
							Si Longitud(docLimpio) <> 8 Y Longitud(docLimpio) <> 11 Entonces
								Escribir "Error: El documento debe tener 8 o 11 caracteres."
							Sino
								Escribir "Remitente: "
								Leer rem
								Escribir "Asunto: "
								Leer asu
								Escribir "Area de destino: "
								Leer area
								
								codigos[totalReg] <- codLimpio
								documentos[totalReg] <- docLimpio
								remitentes[totalReg] <- rem
								asuntos[totalReg] <- asu
								areas[totalReg] <- area
								totalReg <- totalReg + 1
								
								Escribir "Expediente registrado con exito."
							FinSi
						FinSi
					FinSi
				FinSi
				
			2:
				Limpiar Pantalla
				Escribir "--- CONSULTAR EXPEDIENTE ---"
				Escribir "Ingrese codigo a buscar: "
				Leer busq
				busq <- Mayusculas(busq)
				
				pos <- -1
				Para i <- 0 Hasta totalReg - 1 Hacer
					Si codigos[i] = busq Entonces
						pos <- i
					FinSi
				FinPara
				
				Si pos <> -1 Entonces
					Escribir "Expediente encontrado:"
					Escribir "Codigo    : ", codigos[pos]
					Escribir "Documento : ", documentos[pos]
					Escribir "Remitente : ", remitentes[pos]
					Escribir "Asunto    : ", asuntos[pos]
					Escribir "Area      : ", areas[pos]
				Sino
					Escribir "No se encontro ningun expediente con ese codigo."
				FinSi
				
			3:
				Limpiar Pantalla
				Escribir "--- ORDENAR EXPEDIENTES ---"
				Si totalReg <= 1 Entonces
					Escribir "No hay suficientes registros para ordenar."
				Sino
					Para i <- 0 Hasta totalReg - 2 Hacer
						Para j <- 0 Hasta totalReg - i - 2 Hacer
							Si codigos[j] > codigos[j + 1] Entonces
								tempCod <- codigos[j]; codigos[j] <- codigos[j + 1]; codigos[j + 1] <- tempCod
								tempDoc <- documentos[j]; documentos[j] <- documentos[j + 1]; documentos[j + 1] <- tempDoc
								tempRem <- remitentes[j]; remitentes[j] <- remitentes[j + 1]; remitentes[j + 1] <- tempRem
								tempAsu <- asuntos[j]; asuntos[j] <- asuntos[j + 1]; asuntos[j + 1] <- tempAsu
								tempArea <- areas[j]; areas[j] <- areas[j + 1]; areas[j + 1] <- tempArea
							FinSi
						FinPara
					FinPara
					Escribir "Expedientes ordenados con exito."
				FinSi
				
			4:
				Limpiar Pantalla
				Escribir "--- LISTADO DE EXPEDIENTES ---"
				Si totalReg = 0 Entonces
					Escribir "No hay expedientes registrados."
				Sino
					Para i <- 0 Hasta totalReg - 1 Hacer
						Escribir (i + 1), ". ", codigos[i], " | Doc: ", documentos[i], " | Remitente: ", remitentes[i], " | Area: ", areas[i]
					FinPara
				FinSi
				
			5:
				Escribir "Saliendo del prototipo..."
				
			De Otro Modo:
				Escribir "Opcion no valida. Elija entre 1 y 5."
		FinSegun
		
		Si opcion <> 5 Entonces
			Escribir "Presione una tecla para continuar..."
			Esperar Tecla
		FinSi
	Hasta Que opcion = 5
FinAlgoritmo 
