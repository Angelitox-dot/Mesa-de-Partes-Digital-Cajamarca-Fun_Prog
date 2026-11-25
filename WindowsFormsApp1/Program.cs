using System;
using System.Drawing;
using System.Windows.Forms;

namespace EP
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormPrincipal());
        }
    }

    public class FormPrincipal : Form
    {
        // Arreglos paralelos para almacenar los datos del trámite documentario (Tema 4.3)
        static string[] codigos = new string[100];
        static string[] documentos = new string[100];
        static string[] remitentes = new string[100];
        static string[] asuntos = new string[100];
        static string[] areas = new string[100];
        static int totalRegistros = 0;

        // Controles
        TextBox txtCodigo, txtDocumento, txtRemitente, txtAsunto, txtArea, txtBuscar;
        DataGridView grid;
        Label lblEstado;

        public FormPrincipal()
        {
            ConstruirInterfaz();
            RefrescarGrid();
        }

        // ---------------------------------------------------------------
        //  INTERFAZ
        // ---------------------------------------------------------------
        private void ConstruirInterfaz()
        {
            Text = "Sistema Mesa de Partes Digital (EP)";
            ClientSize = new Size(900, 560);
            MinimumSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9.5f);
            BackColor = Color.FromArgb(245, 247, 250);

            // Encabezado
            Panel header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(30, 64, 120) };
            Label titulo = new Label
            {
                Text = "MESA DE PARTES DIGITAL",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 14)
            };
            header.Controls.Add(titulo);
            Controls.Add(header);

            // Barra de estado
            lblEstado = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(225, 230, 238),
                Padding = new Padding(10, 0, 0, 0),
                Text = "Listo."
            };
            Controls.Add(lblEstado);

            // ---- Grupo: Registro ----
            GroupBox gbRegistro = new GroupBox
            {
                Text = "Registrar nuevo expediente",
                Location = new Point(15, 75),
                Size = new Size(320, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            txtCodigo = AgregarCampo(gbRegistro, "Código (Ej: EXP-001)", 25);
            txtDocumento = AgregarCampo(gbRegistro, "DNI (8) o RUC (11)", 85);
            txtRemitente = AgregarCampo(gbRegistro, "Remitente", 145);
            txtAsunto = AgregarCampo(gbRegistro, "Asunto", 205);
            txtArea = AgregarCampo(gbRegistro, "Área de destino", 265);

            Button btnRegistrar = CrearBoton("Registrar", new Point(15, 335), new Size(135, 36), Color.FromArgb(34, 139, 84));
            btnRegistrar.Click += BtnRegistrar_Click;
            Button btnLimpiar = CrearBoton("Limpiar", new Point(165, 335), new Size(135, 36), Color.FromArgb(110, 120, 135));
            btnLimpiar.Click += (s, e) => LimpiarCampos();
            gbRegistro.Controls.Add(btnRegistrar);
            gbRegistro.Controls.Add(btnLimpiar);
            Controls.Add(gbRegistro);

            // ---- Grupo: Consulta y acciones ----
            GroupBox gbConsulta = new GroupBox
            {
                Text = "Consultar y ordenar",
                Location = new Point(350, 75),
                Size = new Size(535, 75),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Label lblBuscar = new Label { Text = "Código:", AutoSize = true, Location = new Point(12, 33) };
            txtBuscar = new TextBox { Location = new Point(70, 29), Width = 130 };
            Button btnBuscar = CrearBoton("Buscar", new Point(210, 26), new Size(85, 30), Color.FromArgb(30, 64, 120));
            btnBuscar.Click += BtnBuscar_Click;
            Button btnOrdenar = CrearBoton("Ordenar (Burbuja)", new Point(305, 26), new Size(125, 30), Color.FromArgb(200, 120, 20));
            btnOrdenar.Click += BtnOrdenar_Click;
            Button btnListar = CrearBoton("Listar todos", new Point(438, 26), new Size(88, 30), Color.FromArgb(110, 120, 135));
            btnListar.Click += (s, e) =>
            {
                RefrescarGrid();
                Estado(totalRegistros == 0 ? "No hay expedientes registrados." : "Mostrando " + totalRegistros + " expediente(s).");
            };
            gbConsulta.Controls.AddRange(new Control[] { lblBuscar, txtBuscar, btnBuscar, btnOrdenar, btnListar });
            Controls.Add(gbConsulta);

            // ---- Tabla ----
            grid = new DataGridView
            {
                Location = new Point(350, 160),
                Size = new Size(535, 315),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            grid.Columns.Add("colNro", "N°");
            grid.Columns.Add("colCodigo", "Código");
            grid.Columns.Add("colDoc", "Documento");
            grid.Columns.Add("colRem", "Remitente");
            grid.Columns.Add("colAsunto", "Asunto");
            grid.Columns.Add("colArea", "Área");
            grid.Columns["colNro"].FillWeight = 30;
            grid.Columns["colCodigo"].FillWeight = 60;
            grid.Columns["colDoc"].FillWeight = 70;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 64, 120);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 250);
            Controls.Add(grid);

            // Botón de salir
            Button btnSalir = CrearBoton("Salir", new Point(15, 490), new Size(320, 36), Color.FromArgb(180, 50, 50));
            btnSalir.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSalir.Click += (s, e) => Close();
            Controls.Add(btnSalir);

            // Enter en el cuadro de búsqueda busca
            txtBuscar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BtnBuscar_Click(s, e); }
            };

            // El header debe quedar arriba de todo
            header.BringToFront();
        }

        private TextBox AgregarCampo(GroupBox padre, string etiqueta, int y)
        {
            Label lbl = new Label { Text = etiqueta, AutoSize = true, Location = new Point(15, y) };
            TextBox txt = new TextBox { Location = new Point(15, y + 22), Width = 285 };
            padre.Controls.Add(lbl);
            padre.Controls.Add(txt);
            return txt;
        }

        private Button CrearBoton(string texto, Point pos, Size tam, Color color)
        {
            Button b = new Button
            {
                Text = texto,
                Location = pos,
                Size = tam,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        // ---------------------------------------------------------------
        //  EVENTOS
        // ---------------------------------------------------------------
        private void BtnRegistrar_Click(object sender, EventArgs e)
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

        private void BtnBuscar_Click(object sender, EventArgs e)
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

        private void BtnOrdenar_Click(object sender, EventArgs e)
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
    }
}
