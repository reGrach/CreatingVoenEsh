using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingVoenEsh.MillitaryData
{
    public class UpdateDb
    {
        //Возврат обневленного списка бригад
        public List<string> UpdateBrigade()
        {
            List<string> brigades = new List<string>();
            using (MillitaryDbContext context = new MillitaryDbContext())
            {
                foreach (Brigade br in context.Brigades)
                {
                    brigades.Add(br.Name);
                }
            }
            return brigades;
        }

        //Возврат обневленного списка батальонов (как список от бригады)
        public DataTable UpdateBattalions(string bridName)
        {
            DataTable tableBat = new DataTable();
            tableBat.Columns.Add("Наименование батальона");
            tableBat.Columns.Add("Погрузка");
            tableBat.Columns.Add("Офицеры");
            tableBat.Columns.Add("Сержанты");
            tableBat.Columns.Add("Солдаты");
            tableBat.Columns.Add("Гражданский персонал");
            tableBat.Columns["Погрузка"].DataType = typeof(bool);
            MillitaryDbContext context = new MillitaryDbContext();
            List<Battalion> batList = new List<Battalion>();
            foreach (Brigade br in context.Brigades)
            {
                if (br.Name == bridName)
                {
                    batList = br.Battalions;
                    break;
                }
            }
            Battalion battalions = new Battalion();
            foreach(Battalion bat in batList)
            {
                tableBat.Rows.Add(bat.Name, bat.Load, bat.Officer, bat.Serjant, 
                    bat.Soldat, bat.Civilian);
            }
            
            return tableBat;
        }




        //Возврат обневленного списка батальонов (как список от бригады)
        public List<string> UpdateListNameBattalions()
        {
            List<string> listBat = new List<string>();
            MillitaryDbContext context = new MillitaryDbContext();
            foreach (Battalion bat in context.Battalions)
            {
                listBat.Add(bat.Name);
            }
            return listBat;
        }

        //Возврат обневленного списка вооружения и военной техники
        public DataTable UpdateEquipments()
        {
            DataTable tableEquip = new DataTable();
            tableEquip.Columns.Add("Шифр");
            tableEquip.Columns.Add("Полное наименование");
            tableEquip.Columns.Add("Длина, [мм]");
            tableEquip.Columns.Add("Масса, [кг]");
            tableEquip.Columns.Add("Установка на сцеп");
            tableEquip.Columns["Установка на сцеп"].DataType = typeof(bool);
            MillitaryDbContext context = new MillitaryDbContext();
            foreach (Equipment equip in context.Equipments)
            {
                tableEquip.Rows.Add(equip.Name, equip.FullName, equip.Lenght,
                    equip.Mass, equip.TypeEquip);
            }
            return tableEquip;
        }

        //Возврат обневленного списка вооружения и военной техники
        public List<string> UpdateListEquipment()
        {
            DataTable newEquipDb = UpdateEquipments();
            List<string> equipList = new List<string>();
            foreach (DataRow ii in newEquipDb.Rows)
            {
                equipList.Add(ii.ItemArray[0].ToString());
            }
            return equipList;
        }

        //Удаление бригады и всех записей о ней
        public void DelBrigade(string textKey)
        {
            MillitaryDbContext context = new MillitaryDbContext();
            var foundBrig = from brig in context.Brigades
                           where brig.Name == textKey
                           select brig;
            context.Brigades.Remove(foundBrig.First());
            context.SaveChanges();
        }

        //Удаление батальона и всех записей о нем
        public void DelBattalion(string textKey)
        {
            MillitaryDbContext context = new MillitaryDbContext();
            var foundBatt = from bat in context.Battalions
                            where bat.Name == textKey
                            select bat;
            context.Battalions.Remove(foundBatt.First());
            context.SaveChanges();
        }

        //Удаление техники и всех записей о ней
        public void DelEquip(string textKey)
        {
            MillitaryDbContext context = new MillitaryDbContext();
            var foundEq = from eq in context.Equipments
                            where eq.Name == textKey
                            select eq;
            context.Equipments.Remove(foundEq.First());
            context.SaveChanges();
        }
    }
}
