using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CreatingVoenEsh.MillitaryData;
using CreatingVoenEsh.VESH;
using static CreatingVoenEsh.VESH.BuildVesh;

namespace CreatingVoenEsh
{
    public partial class MainWind : Form
    {
        public MainWind()
        {
            InitializeComponent();
        }

        //Начальная загрузка базы данных
        private void MainWind_Load(object sender, EventArgs e)
        {
            UpdateDb update = new UpdateDb();
            //Загрузка бригад
            chooseBrig1.DataSource = update.UpdateBrigade();
            changeBrig4Batt.DataSource = update.UpdateBrigade();
            changeBrig4Equip.DataSource = update.UpdateBrigade();
            lookAllElement.DataSource = update.UpdateBrigade();
            //Загрузка батальонов
            UpdateDataGridViewBattalions(chooseBrig1.Text);
            UpdatechangeBrig4Equip(changeBrig4Equip.Text);
            //Загрузка техники
            chooseElement_SelectedIndexChanged(sender, e);
            lookAllEquip.DataSource = update.UpdateListEquipment();

        }


        //////////ДОБАВЛЕНИЕ В БАЗУ ДАННЫХ\\\\\\\\\\\\\\\\

        // ДОБАВИТЬ БРИГАДУ
        private void addBrigabe_Click(object sender, EventArgs e)
        {
            using (MillitaryDbContext context = new MillitaryDbContext())
            {
                foreach (Brigade br in context.Brigades)
                {
                    if (br.Name == newNameBrigabe.Text)
                    {
                        MessageBox.Show("Бригада с такми названием уже существует!\nИзмените название и попробуйте еще раз!");
                        return;
                    }
                }
                Brigade brigade = new Brigade() { Name = newNameBrigabe.Text };
                context.Brigades.Add(brigade);
                context.SaveChanges();
            }
            // Обновление источника данных
            newNameBrigabe.Clear();
            UpdateDb update = new UpdateDb();
            chooseBrig1.DataSource = update.UpdateBrigade();
            changeBrig4Batt.DataSource = update.UpdateBrigade();
            changeBrig4Equip.DataSource = update.UpdateBrigade();
            MessageBox.Show("Бригада успешно добавлена");
        }

        //ДОБАВИТЬ БАТАЛЬОН
        private void addBattalion_Click(object sender, EventArgs e)
        {
            using (MillitaryDbContext context = new MillitaryDbContext())
            {
                foreach (Battalion bat in context.Battalions)
                {
                    if (bat.Name == newNameBattalion.Text)
                    {
                        MessageBox.Show("Батальон с такми названием уже существует!\nИзмените название и попробуйте еще раз!");
                        return;
                    }
                }
                Battalion battalion = new Battalion();
                battalion.Name = newNameBattalion.Text;
                battalion.Load = false;
                battalion.Officer = Convert.ToInt32(newOfficer.Text.ToString());
                battalion.Serjant = Convert.ToInt32(newSerjant.Text);
                battalion.Soldat = Convert.ToInt32(newSoldat.Text);
                battalion.Civilian = Convert.ToInt32(newCivilan.Text);
                // Определяем ID выбранной бригады
                foreach (Brigade br in context.Brigades)
                {
                    if (br.Name == changeBrig4Batt.Text)
                    {
                        battalion.BrigadeId = br.BrigadeId;
                        break;
                    }
                }
                context.Battalions.Add(battalion);
                context.SaveChanges();
            }
            //// Обновление источника данных
            UpdateDataGridViewBattalions(chooseBrig1.Text);
            UpdatechangeBrig4Equip(changeBrig4Equip.Text);
            newNameBattalion.Text = ""; newOfficer.Text = "0"; newSerjant.Text = "0";
            newSoldat.Text = "0"; newCivilan.Text = "0";
            UpdateDb update = new UpdateDb();
            MessageBox.Show("Батальон успешно добавлен");
        }

        //ДОБАВИТЬ ТЕХНИКУ
        private void addEquip_Click(object sender, EventArgs e)
        {
            using (MillitaryDbContext context = new MillitaryDbContext())
            {
                Equipment equipment = new Equipment();
                equipment.Name = newNameEquip.Text;
                equipment.FullName = newFullNameEquip.Text;
                equipment.Lenght = Convert.ToInt32(newLenghtEquip.Text);
                equipment.Mass = Convert.ToInt32(newMassEquip.Text);
                equipment.TypeEquip = typeEquip.Checked;
                context.Equipments.Add(equipment);
                context.SaveChanges();
            }
            // Обновление источника данных
            UpdateDb up = new UpdateDb();
            lookAllEquip.DataSource = up.UpdateListEquipment();
            newNameEquip.Text = ""; newFullNameEquip.Text = ""; newLenghtEquip.Text = "0";
            newMassEquip.Text = "0";
            MessageBox.Show("Техника успешно добавлена");
        }

        //ДОБАВИТЬ ТЕХНИКУ В БАТАЛЬОН
        //Заносим технику в промежуточный список для создания коллекции в батальоне
        private void chooseEquip_Click(object sender, EventArgs e)
        {
            if (dataGridViewAddEquip.RowCount != 1)
            {
                for (int i = 0; i < dataGridViewAddEquip.RowCount - 1; i++)
                {
                    if (dataGridViewAddEquip[0, i].Value.ToString() == lookAllEquip.SelectedValue.ToString())
                    {
                        MessageBox.Show("Техника с таким шифром уже добавлена!");
                        return;
                    }
                }
            }
            dataGridViewAddEquip.Rows.Add(lookAllEquip.SelectedItem);
        }
        //Связываем технику и батальон
        private void updateBatt_Click(object sender, EventArgs e)
        {
            MillitaryDbContext context = new MillitaryDbContext();
            var foundBat = from bat in context.Battalions
                           where bat.Name == chooseBat4Equip.Text
                           select bat;
            //Создаем и заполняем список ВВТ для батальона
            List<EquipmentBattalion> listEquip = new List<EquipmentBattalion>();
            while (dataGridViewAddEquip.RowCount != 1)
            {
                string newName = dataGridViewAddEquip[0, 0].Value.ToString();
                foreach (Equipment eq in context.Equipments)
                {
                    if (eq.Name == newName)
                    {
                        EquipmentBattalion eq2bat = new EquipmentBattalion()
                        {
                            Battalions = foundBat.First(),
                            Equipments = eq,
                            Count = Convert.ToInt32(dataGridViewAddEquip[1, 0].Value)
                        };
                        listEquip.Add(eq2bat);
                        break;
                    }
                }
                dataGridViewAddEquip.Rows.RemoveAt(0);
            }
            context.EquipmentBattalions.AddRange(listEquip);
            context.SaveChanges();
            MessageBox.Show("Техника успешно добавлена в батальон");
        }

        //////////УДАЛЕНИЕ ИЗ БАЗЫ ДАННЫХ\\\\\\\\\\\\\\\\
        //Выбор списка элементов одной из таблиц
        private void chooseElement_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDb up = new UpdateDb();
            switch (chooseElement.SelectedIndex)
            {
                case 0:
                    lookAllElement.DataSource = up.UpdateBrigade();
                    break;
                case 1:
                    lookAllElement.DataSource = up.UpdateListNameBattalions();
                    break;
                case 2:
                    lookAllElement.DataSource = up.UpdateListEquipment(); ;
                    break;
            }
            lookAllElement.DisplayMember = "Name";
        }
        //Поиск элементов по алфавиту
        private void inputFind_TextChanged(object sender, EventArgs e)
        {
            chooseElement_SelectedIndexChanged(chooseElement, e);
            List<string> listEl = new List<string>();
            string text = inputFind.Text;
            foreach (string aa in lookAllElement.Items)
            {
                if (aa.Contains(text))
                {
                    listEl.Add(aa);
                }
            }
            lookAllElement.DataSource = listEl;
        }
        //Удаление выбранного элемента
        private void delElement_Clic(object sender, EventArgs e)
        {
            UpdateDb up = new UpdateDb();
            string text = lookAllElement.SelectedItem.ToString();
            switch (chooseElement.SelectedIndex)
            {
                case 0:
                    DialogResult resQwest1 = MessageBox.Show("Вы уверены, что хотите удалить выбранную бригаду и все ее подразделения?", "Предупреждение", MessageBoxButtons.YesNo);
                    if (resQwest1 == DialogResult.Yes) up.DelBrigade(text);
                    MainWind_Load(sender, e);
                    break;
                case 1:
                    DialogResult resQwest2 = MessageBox.Show("Вы уверены, что хотите удалить выбранный батальон?", "Предупреждение", MessageBoxButtons.YesNo);
                    if (resQwest2 == DialogResult.Yes) up.DelBattalion(text);
                    MainWind_Load(sender, e);
                    break;
                case 2:
                    DialogResult resQwest3 = MessageBox.Show("Вы уверены, что хотите исключить из батальонов и удалить выбранную технику?", "Предупреждение", MessageBoxButtons.YesNo);
                    if (resQwest3 == DialogResult.Yes) up.DelEquip(text);
                    MainWind_Load(sender, e);
                    break;
            }
        }

        //////////ЗАПОЛНЕНИЕ ЭЛЕМЕНТОВ ИЗ БАЗЫ ДАННЫХ\\\\\\\\\\\\\\\
        //Заполнение DataGridViewBattalions
        private void UpdateDataGridViewBattalions(string bridName)
        {
            UpdateDb update = new UpdateDb();
            DataTable newBatDb = update.UpdateBattalions(bridName);
            battalionsDataGridView.DataSource = newBatDb;
            battalionsDataGridView.Columns[0].Width = 550;
            battalionsDataGridView.Columns[0].ReadOnly = true;
            battalionsDataGridView.Columns[0].Resizable = DataGridViewTriState.False;
            battalionsDataGridView.Columns[1].Width = 80;
            battalionsDataGridView.Columns[1].Resizable = DataGridViewTriState.False;
            battalionsDataGridView.Columns[2].Visible = false;
            battalionsDataGridView.Columns[3].Visible = false;
            battalionsDataGridView.Columns[4].Visible = false;
            battalionsDataGridView.Columns[5].Visible = false;
        }
        //Заполнение chooseBat4Equip
        private void UpdatechangeBrig4Equip(string bridName)
        {
            UpdateDb update = new UpdateDb();
            DataTable newBatDb = update.UpdateBattalions(bridName);
            List<string> batpList = new List<string>();
            foreach (DataRow ii in newBatDb.Rows)
            {
                batpList.Add(ii.ItemArray[0].ToString());
            }
            chooseBat4Equip.DataSource = batpList;
        }
        //Отображение параметров выбранной техники
        private void lookAllEquip_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDb update = new UpdateDb();
            DataTable newEquipDb = update.UpdateEquipments();
            foreach (DataRow row in newEquipDb.Rows)
            {
                if (row.ItemArray[0].ToString() == lookAllEquip.SelectedValue.ToString())
                {
                    showNameEq.Text = row.ItemArray[1].ToString();
                    showLengEq.Text = row.ItemArray[2].ToString();
                    showMassEq.Text = row.ItemArray[3].ToString();
                    if (Convert.ToBoolean(row.ItemArray[4])) showLoadEq.Text = "Возможна";
                    else showLoadEq.Text = "Не возможна";
                }

            }


        }
        //Вывод информаии о батальоне
        private void battalionsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int changeCell = battalionsDataGridView.CurrentCellAddress.X;
            if (changeCell == 0)
            {
                viewOficer.Text = Convert.ToString(this.battalionsDataGridView.CurrentRow.Cells[2].Value);
                viewSerjant.Text = Convert.ToString(this.battalionsDataGridView.CurrentRow.Cells[3].Value);
                viewSoldat.Text = Convert.ToString(this.battalionsDataGridView.CurrentRow.Cells[4].Value);
                viewCivilian.Text = Convert.ToString(this.battalionsDataGridView.CurrentRow.Cells[5].Value);
                dataGridViewEquip.Rows.Clear();
                MillitaryDbContext context = new MillitaryDbContext();
                string qwest = battalionsDataGridView.CurrentRow.Cells[0].Value.ToString();
                var foundBat = from bat in context.Battalions
                               where bat.Name == qwest
                               select bat;
                Battalion battalion = foundBat.First();
                int ii = 0;
                foreach (EquipmentBattalion eq2bat in context.EquipmentBattalions)
                {
                    if (eq2bat.BattalionId == battalion.BattalionId)
                    {
                        dataGridViewEquip.Rows.Add();
                        dataGridViewEquip[0, ii].Value = eq2bat.Equipments.Name;
                        dataGridViewEquip[1, ii].Value = eq2bat.Count;
                        ii++;
                    }
                }



                //List<EquipmentBattalion> eqList = new List<EquipmentBattalion>();
                //eqList = context.EquipmentBattalions.Where(x => x.BattalionId == battalion.BattalionId).ToList();
                //dataGridViewEquip.DataSource = eqList.;
            }
        }
        //Обновление списка батальонов при выборе бригады
        private void chooseBrig1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewBattalions(chooseBrig1.Text);
        }
        //Обновление списка батальонов при выборе бригады
        private void changeBrig4Equip_SelectedIndexChanged(object sender, EventArgs e)
        {
            chooseBat4Equip.Text = "";
            UpdatechangeBrig4Equip(changeBrig4Equip.Text);
        }
        //Добавление батальона в список на погрузку
        private void battalionsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int changeCellCol = battalionsDataGridView.CurrentCellAddress.X;
            int changeCellRow = battalionsDataGridView.CurrentCellAddress.Y;
            if (changeCellCol == 1)
            {
                battalionsDataGridView[0, changeCellRow].Selected = true;
                if (Convert.ToBoolean(battalionsDataGridView[1, changeCellRow].Value))
                {
                    readyBat.Items.Add(battalionsDataGridView[0, changeCellRow].Value.ToString());
                }
                else
                {
                    readyBat.Items.Remove(battalionsDataGridView[0, changeCellRow].Value.ToString());
                }
            }
        }

        //Включаем возможность для локального редактирования батальона
        private void localChange_Click(object sender, EventArgs e)
        {
            viewOficer.ReadOnly = false;
            viewSerjant.ReadOnly = false;
            viewSoldat.ReadOnly = false;
            viewCivilian.ReadOnly = false;
            dataGridViewEquip.Columns[1].ReadOnly = false;
        }

        //////////ФОРМИРОВАНИЕ ВОИНСКИХ ЭШЕЛОНОВ\\\\\\\\\\\\\\\
        private void BuildVEshelon(object sender, EventArgs e)
        {
            //Очищаем поле эшелонов
            tabControl1.TabPages.Clear();
            int ii = 0;
            //Флаг установки на сцеп
            int flagUnistallScep = Convert.ToByte(radioButtonScepFalse.Checked) +
    2 * Convert.ToByte(radioButtonScepTrueMin.Checked) +
    3 * Convert.ToByte(radioButtonScepTrueMax.Checked);
            //Создание списка батальонов
            List<string> list = new List<string>();
            foreach (string nameBat in readyBat.Items) { list.Add(nameBat); }
            //Объявление новых классов
            ClassDraw draw = new ClassDraw()
            {
                widthBitmap = tabControl1.Width,
                heightBitmap = tabControl1.Height
            };
            BuildVesh newCollectVesh = new BuildVesh()
            {
                nameOfBatt = list,
                haveKitchen = flagKitchen.Checked,
                lenghtPlatform = Convert.ToInt32(inputLenPlat.Text),
                buferLenght = Convert.ToInt32(inputLenBufer.Text),
                lenghtScep = Convert.ToInt32(inputLenScep.Text)
            };
            List<BuildnUnit> CollectBatt = newCollectVesh.CreateCollectBatt();
            List<VoinEshelon> CollectVesh = newCollectVesh.CalcBatt4Load(newCollectVesh.CreateCollectBatt(), flagUnistallScep);
            foreach (VoinEshelon vesh in CollectVesh)
            {
                draw.voinEsh = vesh;
                draw.batt = CollectBatt.Where(x => x.nameUnit == vesh.nameVeSh).First();
                //Добавление новой вкладки
                TabPage newVoin = new TabPage()
                {
                    Text = Convert.ToString(++ii) + " воинский эшелон",
                    BackColor = Color.White,
                    Parent = tabControl1
                };
                PictureBox newPict = new PictureBox()
                {
                    Parent = newVoin,
                    Dock = DockStyle.Fill
                };
                newPict.Image = draw.DrawVEshelon();
            }
            butt4build.Text = "Переформировать";
        }
    }
}
