using static WinFormsApp1.Form1;
using System.ComponentModel;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        public class Employee
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Position { get; set; }
            public decimal Salary { get; set; }
        }

        // Хранение данных
        private List<Employee> employees = new List<Employee>();
        private int nextEmployeeId = 1;

        // Элементы управления
        private DataGridView employeesGrid;
        private TabControl tabControl;
        private Button btnAddEmployee;
        private Button btnEditEmployee;
        private Button btnDeleteEmployee;

        public Form1()
        {
            InitializeComponent();
            InitializeData();
            CreateUI();
        }

        private void InitializeData()
        {
            // Добавляем тестовые данные
            employees.Add(new Employee
            {
                Id = nextEmployeeId++,
                FullName = "Иванов Иван Иванович",
                Position = "Менеджер",
                Salary = 50000
            });

            employees.Add(new Employee
            {
                Id = nextEmployeeId++,
                FullName = "Петрова Мария Сергеевна",
                Position = "Разработчик",
                Salary = 75000
            });
        }

        private void CreateUI()
        {
            // Настройка главной формы
            this.Text = "Учет сотрудников";
            this.Size = new Size(800, 600);

            // Создаем TabControl
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            this.Controls.Add(tabControl);

            // Вкладка "Сотрудники"
            var tabEmployees = new TabPage("Сотрудники");
            tabControl.TabPages.Add(tabEmployees);

            // DataGridView для сотрудников
            employeesGrid = new DataGridView();
            employeesGrid.Dock = DockStyle.Fill;
            employeesGrid.AutoGenerateColumns = false;
            employeesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Колонки
            employeesGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "ID",
                DataPropertyName = "Id"
            });

            employeesGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "ФИО",
                DataPropertyName = "FullName",
                Width = 200
            });

            employeesGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Должность",
                DataPropertyName = "Position",
                Width = 150
            });

            employeesGrid.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Зарплата",
                DataPropertyName = "Salary",
                DefaultCellStyle = new DataGridViewCellStyle() { Format = "C2" }
            });

            employeesGrid.DataSource = new BindingList<Employee>(employees);
            tabEmployees.Controls.Add(employeesGrid);

            // Панель кнопок
            var panel = new Panel();
            panel.Dock = DockStyle.Bottom;
            panel.Height = 40;
            tabEmployees.Controls.Add(panel);

            btnAddEmployee = new Button()
            {
                Text = "Добавить",
                Location = new Point(10, 5)
            };
            btnAddEmployee.Click += BtnAddEmployee_Click;

            btnEditEmployee = new Button()
            {
                Text = "Изменить",
                Location = new Point(100, 5)
            };
            btnEditEmployee.Click += BtnEditEmployee_Click;

            btnDeleteEmployee = new Button()
            {
                Text = "Удалить",
                Location = new Point(190, 5)
            };
            btnDeleteEmployee.Click += BtnDeleteEmployee_Click;

            panel.Controls.AddRange(new Control[] {
            btnAddEmployee,
            btnEditEmployee,
            btnDeleteEmployee
        });
        }

        // Обработчики событий для кнопок
        private void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            using (var form = new AddEditEmployeeForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var newEmployee = new Employee
                    {
                        Id = nextEmployeeId++,
                        FullName = form.FullName,
                        Position = form.Position,
                        Salary = form.Salary
                    };
                    employees.Add(newEmployee);
                    RefreshGrid();
                }
            }
        }

        private void BtnEditEmployee_Click(object sender, EventArgs e)
        {
            if (employeesGrid.SelectedRows.Count == 0) return;

            var selectedEmployee = (Employee)employeesGrid.SelectedRows[0].DataBoundItem;
            using (var form = new AddEditEmployeeForm(selectedEmployee))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    selectedEmployee.FullName = form.FullName;
                    selectedEmployee.Position = form.Position;
                    selectedEmployee.Salary = form.Salary;
                    RefreshGrid();
                }
            }
        }

        private void BtnDeleteEmployee_Click(object sender, EventArgs e)
        {
            if (employeesGrid.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Удалить выбранного сотрудника?",
                                "Подтверждение",
                                MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    var employee = (Employee)employeesGrid.SelectedRows[0].DataBoundItem;
                    employees.Remove(employee);
                    RefreshGrid();
                }
            }
        }

        private void RefreshGrid()
        {
            employeesGrid.DataSource = null;
            employeesGrid.DataSource = new BindingList<Employee>(employees);
        }
    }

    // Форма для добавления/редактирования сотрудника
    public class AddEditEmployeeForm : Form
    {
        public string FullName { get; private set; }
        public string Position { get; private set; }
        public decimal Salary { get; private set; }

        private TextBox txtFullName;
        private TextBox txtPosition;
        private NumericUpDown numSalary;

        public AddEditEmployeeForm(Employee employee = null)
        {
            InitializeComponents();

            if (employee != null)
            {
                txtFullName.Text = employee.FullName;
                txtPosition.Text = employee.Position;
                numSalary.Value = employee.Salary;
            }
        }

        private void InitializeComponents()
        {
            this.Text = "Добавление сотрудника";
            this.Size = new Size(300, 200);

            var lblFullName = new Label() { Text = "ФИО:", Top = 20, Left = 10 };
            txtFullName = new TextBox() { Top = 20, Left = 100, Width = 170 };

            var lblPosition = new Label() { Text = "Должность:", Top = 50, Left = 10 };
            txtPosition = new TextBox() { Top = 50, Left = 100, Width = 170 };

            var lblSalary = new Label() { Text = "Зарплата:", Top = 80, Left = 10 };
            numSalary = new NumericUpDown()
            {
                Top = 80,
                Left = 100,
                Width = 170,
                DecimalPlaces = 2,
                Maximum = 1000000
            };

            var btnOk = new Button() { Text = "OK", DialogResult = DialogResult.OK, Top = 120, Left = 100 };
            var btnCancel = new Button() { Text = "Отмена", DialogResult = DialogResult.Cancel, Top = 120, Left = 180 };

            btnOk.Click += (s, e) =>
            {
                if (ValidateInput())
                {
                    FullName = txtFullName.Text;
                    Position = txtPosition.Text;
                    Salary = numSalary.Value;
                    DialogResult = DialogResult.OK;
                }
            };

            this.Controls.AddRange(new Control[] {
            lblFullName, txtFullName,
            lblPosition, txtPosition,
            lblSalary, numSalary,
            btnOk, btnCancel
        });
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО сотрудника");
                return false;
            }

            if (numSalary.Value <= 0)
            {
                MessageBox.Show("Зарплата должна быть больше нуля");
                return false;
            }

            return true;
        }
    }
}
