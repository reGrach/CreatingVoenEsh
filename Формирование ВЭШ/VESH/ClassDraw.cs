using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using static CreatingVoenEsh.VESH.BuildVesh;

namespace CreatingVoenEsh
{
    public class ClassDraw
    {
        //Получение размеров окна рисования
        public int widthBitmap;
        public int heightBitmap;
        public static int lenghtPlatform = 13300;
        //Получение информации о батальоне
        public VoinEshelon voinEsh;
        public BuildnUnit batt;
        //Ввод дополнительных данных
        static int koef = 110;
        int lenWag = lenghtPlatform / koef;
        int wigWag = 40;

        //Рисование Эшелона
        public Bitmap DrawVEshelon()
        {
            Bitmap bmp = new Bitmap(widthBitmap, heightBitmap);
            Graphics text = Graphics.FromImage(bmp);
            text.TextRenderingHint = TextRenderingHint.AntiAlias;
            Graphics graph = Graphics.FromImage(bmp);
            Rectangle rec = new Rectangle();
            SolidBrush brushText = new SolidBrush(Color.Black);
            Font newFont = new Font("Times New Roman", 12, FontStyle.Regular);
            Pen pen = new Pen(Color.Black);
            Point razriv = new Point(1280 / koef + lenWag, 20 + wigWag);
            Point koord = new Point();
            int xx = 0, yy = 0;
            List<EquipInVesh> equips = new List<EquipInVesh>();
            bool flagCentre = false;
            text.DrawString(voinEsh.nameVeSh, new Font("Times New Roman", 14, FontStyle.Bold), brushText, new Point(100, 0));
            //Рисуем состав по вагону без техники
            foreach (WagonInVesh wag in voinEsh.listWagons)
            {
                koord = new Point(10 + xx * razriv.X, 30 + yy * razriv.Y);
                graph.DrawRectangle(pen, koord.X, koord.Y, lenWag, wigWag);
                text.DrawString(wag.numWagon.ToString() + "   " + TypeWag(wag.typeWagon),
                    newFont, brushText, koord.X, koord.Y);
                //Отображаем технику
                equips = voinEsh.listEquips.FindAll(x => x.numWagon == wag.numWagon);
                if (equips.Count == 1 && flagCentre)
                {
                    rec = new Rectangle(koord.X + (lenghtPlatform - equips[0].lengthEq) / (2 * koef),
    koord.Y + 18, equips[0].lengthEq / koef, 20);
                    graph.DrawRectangle(pen, rec);
                    graph.FillRectangle(new SolidBrush(equips[0].color), rec);
                    equips.RemoveAt(0);
                }

                while (equips.Count != 0)
                {
                    rec = new Rectangle(koord.X + equips[0].lenFromBegPl / koef, koord.Y + 18, equips[0].lengthEq / koef, 20);
                    graph.DrawRectangle(pen, rec);
                    graph.FillRectangle(new SolidBrush(equips[0].color), rec);
                    equips.RemoveAt(0);
                    if (equips.Count == 1 && wag.numWagon % 7 == 0 && (lenghtPlatform - equips[0].lenFromBegPl) < equips[0].lengthEq)
                    {
                        Rectangle newRec = new Rectangle(koord.X + equips[0].lenFromBegPl / koef,
                           koord.Y + 18, equips[0].lengthEq / (2 * koef), 20);
                        graph.DrawRectangle(pen, newRec);
                        graph.FillRectangle(new SolidBrush(equips[0].color), newRec);
                        //Рисуем переход
                        graph.FillEllipse(new SolidBrush(equips[0].color),
                            new Rectangle(koord.X + equips[0].lenFromBegPl / koef + equips[0].lengthEq / (2 * koef) - 5,
                            koord.Y + 18, 10, 10));
                        graph.FillEllipse(new SolidBrush(Color.White),
                            new Rectangle(koord.X + equips[0].lenFromBegPl / koef + equips[0].lengthEq / (2 * koef) - 5,
                            koord.Y + 28, 10, 10));
                        //Непосредственно переход к следующей строчке
                        Rectangle newRec2 = new Rectangle(5, 30 + (yy + 1) * razriv.Y + 18, equips[0].lengthEq / (2 * koef), 20);
                        graph.DrawRectangle(pen, newRec2);
                        graph.FillRectangle(new SolidBrush(equips[0].color), newRec2);
                        //Рисуем переход
                        graph.FillEllipse(new SolidBrush(Color.White),
                            new Rectangle(0, 30 + (yy + 1) * razriv.Y + 18, 10, 10));
                        graph.FillEllipse(new SolidBrush(equips[0].color),
                            new Rectangle(0, 30 + (yy + 1) * razriv.Y + 28, 10, 10));
                        equips.RemoveAt(0);
                        flagCentre = false;
                    }
                }

                xx++;
                if (wag.numWagon % 7 == 0) { xx = 0; yy++; }
            }
            xx = 0; yy = 0;
            //Рисуем условные обозначения
            Point koordName = new Point(950, 50);
            foreach (EquipInBatt eqBatt in batt.listEquip)
            {
                rec = new Rectangle(new Point(koordName.X, koordName.Y), new Size(50, 20));
                graph.DrawRectangle(pen, rec);
                graph.FillRectangle(new SolidBrush(eqBatt.color), rec);
                text.DrawString(" - " + eqBatt.codeEq, newFont, brushText, koordName.X + 55, koordName.Y);
                koordName.Y = koordName.Y + 30;
            }
            return bmp;
        }

        //Конвертация имени типа платформы
        private string TypeWag(int a)
        {
            switch (a)
            {
                case 1:
                    return "Платформа";
                case 2:
                    return "Купе";
                case 3:
                    return "Плацкарт";
                case 4:
                    return "Кухня";
                default:
                    return "Ошибка";
            }
        }
    }
}
