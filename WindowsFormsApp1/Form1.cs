using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // Arreglos paralelos para almacenar los datos del trámite documentario (Tema 4.3)
        static string[] codigos = new string[100];
        static string[] documentos = new string[100];
        static string[] remitentes = new string[100];
        static string[] asuntos = new string[100];
        static string[] areas = new string[100];
        static int totalRegistros = 0;

        public Form1()
        {
            InitializeComponent();

            // Estilo de la tabla (se aplica al ejecutar)
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 120);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 250);

            RefrescarGrid();
        }

        // ---------------------------------------------------------------
        //  EVENTOS
        // ---------------------------------------------------------------
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                string codValidado = ValidarCodigoExpediente(txtCodigo.Text);
                string docValidado = ValidarDocumento(txtDocumento.Text);

                string remitente = txtRemitente.Text.Trim();
                string asunto = txtAsunto.Text.Trim();
                string area = txtArea.Text.Trim();

                if (remitente == "" || asunto == "" || area == "")
                    throw new Exception("Remitente, asunto y área no pueden estar vacíos.");

                if (Registrar(codValidado, docValidado, remitente, asunto, area))
                {
                    RefrescarGrid();
                    LimpiarCampos();
                    Estado("¡Expediente " + codValidado + " registrado con éxito!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Dato inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text.Trim().ToUpper();
            if (busqueda == "")
            {
                MessageBox.Show("Ingrese el código a buscar.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int indice = BuscarSecuencial(busqueda);
            if (indice != -1)
            {
                grid.ClearSelection();
                grid.Rows[indice].Selected = true;
                grid.CurrentCell = grid.Rows[indice].Cells[0];

                MessageBox.Show(
                    "Código    : " + codigos[indice] + "\n" +
                    "Documento : " + documentos[indice] + "\n" +
                    "Remitente : " + remitentes[indice] + "\n" +
                    "Asunto    : " + asuntos[indice] + "\n" +
                    "Área      : " + areas[indice],
                    "¡EXPEDIENTE ENCONTRADO!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Estado("Expediente " + busqueda + " encontrado.");
            }
            else
            {
                MessageBox.Show("No se encontró ningún registro con ese código.", "Consulta",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Estado("Sin resultados para " + busqueda + ".");
            }
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnBuscar_Click(sender, e);
            }
        }

        private void btnOrdenar_Click(object sender, EventArgs e)
        {
            if (totalRegistros <= 1)
            {
                MessageBox.Show("No hay suficientes registros para ordenar.", "Ordenamiento",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OrdenarPorCodigos();
            RefrescarGrid();
            Estado("¡Expedientes ordenados alfabéticamente por código!");
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            RefrescarGrid();
            Estado(totalRegistros == 0
                ? "No hay expedientes registrados."
                : "Mostrando " + totalRegistros + " expediente(s).");
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ---------------------------------------------------------------
        //  UTILIDADES DE LA INTERFAZ
        // ---------------------------------------------------------------
        private void RefrescarGrid()
        {
            grid.Rows.Clear();
            for (int i = 0; i < totalRegistros; i++)
            {
                grid.Rows.Add(i + 1, codigos[i], documentos[i], remitentes[i], asuntos[i], areas[i]);
            }
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtDocumento.Clear();
            txtRemitente.Clear();
            txtAsunto.Clear();
            txtArea.Clear();
            txtCodigo.Focus();
        }

        private void Estado(string mensaje)
        {
            lblEstado.Text = mensaje;
        }

        // ---------------------------------------------------------------
        //  VALIDACIÓN DE CADENAS (Tema 4.2)
        // ---------------------------------------------------------------
        public static string ValidarCodigoExpediente(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                throw new Exception("El código no puede estar vacío.");

            string codigoLimpiado = texto.Trim().ToUpper();
            string[] partes = codigoLimpiado.Split('-');

            if (partes.Length != 2 || partes[0] != "EXP" || partes[1].Length != 3)
                throw new Exception("Formato inválido. Debe ser EXP-### (Ej: EXP-001).");

            foreach (char c in partes[1])
            {
                if (!char.IsDigit(c))
                    throw new Exception("La parte numérica del código debe contener solo dígitos.");
            }

            return codigoLimpiado;
        }

        public static string ValidarDocumento(string doc)
        {
            if (string.IsNullOrEmpty(doc))
                throw new Exception("El documento no puede estar vacío.");

            string docLimpio = doc.Trim();

            if (docLimpio.Length != 8 && docLimpio.Length != 11)
                throw new Exception("El documento debe tener 8 dígitos (DNI) u 11 dígitos (RUC).");

            foreach (char c in docLimpio)
            {
                if (!char.IsDigit(c))
                    throw new Exception("El documento solo debe contener números.");
            }

            return docLimpio;
        }

        // ---------------------------------------------------------------
        //  GESTIÓN CON ARREGLOS PARALELOS Y BURBUJA (Temas 3.2, 4.3, 4.4)
        // ---------------------------------------------------------------
        public static bool Registrar(string cod, string doc, string rem, string asu, string are)
        {
            if (totalRegistros >= 100)
            {
                MessageBox.Show("Capacidad máxima del sistema alcanzada.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (BuscarSecuencial(cod) != -1)
            {
                MessageBox.Show("El código " + cod + " ya se encuentra registrado.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            codigos[totalRegistros] = cod;
            documentos[totalRegistros] = doc;
            remitentes[totalRegistros] = rem;
            asuntos[totalRegistros] = asu;
            areas[totalRegistros] = are;

            totalRegistros++;
            return true;
        }

        public static int BuscarSecuencial(string codigoBuscado)
        {
            for (int i = 0; i < totalRegistros; i++)
            {
                if (codigos[i] == codigoBuscado)
                    return i;
            }
            return -1;
        }

        public static void OrdenarPorCodigos()
        {
            for (int i = 0; i < totalRegistros - 1; i++)
            {
                for (int j = 0; j < totalRegistros - i - 1; j++)
                {
                    if (codigos[j].CompareTo(codigos[j + 1]) > 0)
                    {
                        string tempCod = codigos[j];
                        codigos[j] = codigos[j + 1];
                        codigos[j + 1] = tempCod;

                        string tempDoc = documentos[j];
                        documentos[j] = documentos[j + 1];
                        documentos[j + 1] = tempDoc;

                        string tempRem = remitentes[j];
                        remitentes[j] = remitentes[j + 1];
                        remitentes[j + 1] = tempRem;

                        string tempAsu = asuntos[j];
                        asuntos[j] = asuntos[j + 1];
                        asuntos[j + 1] = tempAsu;

                        string tempAre = areas[j];
                        areas[j] = areas[j + 1];
                        areas[j + 1] = tempAre;
                    }
                }
            }
        }

        private void gbRegistro_Enter(object sender, EventArgs e)
        {

        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {

        }

        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }
    }
}