using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CreatingVoenEsh.MillitaryData;

namespace CreatingVoenEsh.VESH
{
    public class BuildVesh
    {
        //Чтение данных из окошка
        public List<string> nameOfBatt;
        //Массо-габаритные параметры платформы
        public int lenghtPlatform;
        public int lenghtScep;
        public int buferLenght;
        public int maxMassPlatform = 60000;
        //Число мест в вагонах
        public int sizeKupe = 40;
        public int sizePlatcart = 54;
        //Ниличие кухни
        public bool haveKitchen;

        // Создание коллекции батальонов выбранных для погрузки
        public List<BuildnUnit> CreateCollectBatt()//List<string> nameOfBatt)
        {
            List<BuildnUnit> collBatt4Load = new List<BuildnUnit>();
            MillitaryDbContext context = new MillitaryDbContext();
            int ii = 0;
            foreach (string batt in nameOfBatt)
            {
                var foundBat = from dumb in context.Battalions
                               where dumb.Name == batt
                               select dumb;
                Battalion battalion = foundBat.First();
                List<EquipInBatt> collEquip = new List<EquipInBatt>();
                //List<EquipInBatt> collEquipSpec = new List<EquipInBatt>();
                //List<EquipInBatt> collEquipUsual = new List<EquipInBatt>();
                int rgb = 1;
                foreach (EquipmentBattalion eq2bat in context.EquipmentBattalions)
                {
                    if (eq2bat.BattalionId == battalion.BattalionId)
                    {
                        //Задаем новый цвет
                        Color newColor = new Color();
                        newColor = DefineColor(rgb++);
                        EquipInBatt equip = new EquipInBatt(eq2bat.Equipments.Name, newColor, eq2bat.Equipments.Lenght,
                            eq2bat.Equipments.Mass, eq2bat.Count, eq2bat.Equipments.TypeEquip);
                        //Отделяем специальную технику от обычной
                        //if (eq2bat.Equipments.TypeEquip) collEquipUsual.Add(equip);
                        //else collEquipSpec.Add(equip);
                        collEquip.Add(equip);
                    }
                }
                BuildnUnit newUnit = new BuildnUnit(++ii, battalion.Name, battalion.Brigade.Name,
                    battalion.Officer, battalion.Serjant, battalion.Soldat, battalion.Civilian, collEquip); //collEquipSpec, collEquipUsual);
                collBatt4Load.Add(newUnit);
            }
            return collBatt4Load;
        }

        //Расчет в потребности подвижного состава для подразделения
        public List<VoinEshelon> CalcBatt4Load(List<BuildnUnit> CollBatt, int flagUnistallScep)
        {
            List<VoinEshelon> CollEshelon = new List<VoinEshelon>();
            int freePlaceWagon = lenghtPlatform;
            double needKupe, needPlackart, needWag4Person;
            bool flagType = false, flagPers = true;
            //Перебор батальонов предназначеных на погрузку
            foreach (BuildnUnit oneBatt in CollBatt)
            {
                bool flagVesh = true;
                flagPers = true;
                while (flagVesh)
                {
                    //Ввод дополнительных переменных
                    int ii = 1, maxLenEq, LenEq = 0, indMaxLenEq;
                    //Создание новых коллекций ВВТ и вагонов
                    List<EquipInVesh> listEqs = new List<EquipInVesh>();
                    List<WagonInVesh> listWags = new List<WagonInVesh>();
                    //Потребность в вагонах для личного состава
                    needKupe = Math.Ceiling((double)(oneBatt.numOficer + oneBatt.numGP) / sizeKupe);
                    needPlackart = Math.Ceiling((double)(oneBatt.numSerjant + oneBatt.numSoldat) / sizePlatcart);
                    needWag4Person = needKupe + needPlackart + Convert.ToInt32(haveKitchen);
                    //Заполнение эшелонов техникой
                    while (oneBatt.listEquip.Count != 0)
                    {
                        if (ii == 57) { flagPers = false; break; }
                        WagonInVesh wagon = new WagonInVesh() { numWagon = ii++, typeWagon = 1 };
                        if (wagon.numWagon > 10 && freePlaceWagon == lenghtPlatform && needWag4Person != 0 && flagPers) // && Закончились вагоны с людьми )
                        {
                            if (needKupe != 0)
                            {
                                wagon.typeWagon = 2;
                                needKupe--; needWag4Person--;
                            }
                            else if (needPlackart != 0)
                            {
                                wagon.typeWagon = 3;
                                needPlackart--; needWag4Person--;
                            }
                            else
                            {
                                wagon.typeWagon = 4;
                                needWag4Person--;
                            }
                            listWags.Add(wagon);
                            continue;
                        }

                        //Опредляем тип техники, с которой будем работать
                        if (oneBatt.listEquip.Where(x => x.typeEq == false && x.lengthEq <= freePlaceWagon).Count() != 0) flagType = false;
                        else if (oneBatt.listEquip.Where(x => x.typeEq == true && x.lengthEq <= freePlaceWagon).Count() != 0) flagType = true;
                        //Ищем технику по флагу с максимальной длиной
                        maxLenEq = oneBatt.listEquip.Where(x => x.typeEq == flagType && x.lengthEq <= freePlaceWagon).Max(y => y.lengthEq);
                        indMaxLenEq = oneBatt.listEquip.FindIndex(x => x.lengthEq == maxLenEq && x.typeEq == flagType);
                        //Заполняем структуру техники
                        EquipInVesh equip = new EquipInVesh()
                        {
                            codeEq = oneBatt.listEquip[indMaxLenEq].codeEq,
                            color = oneBatt.listEquip[indMaxLenEq].color,
                            lenFromBegPl = lenghtPlatform - freePlaceWagon,
                            lengthEq = oneBatt.listEquip[indMaxLenEq].lengthEq,
                            numWagon = wagon.numWagon
                        };
                        //Вычисляем остаток длины на платформе
                        freePlaceWagon = freePlaceWagon - oneBatt.listEquip[indMaxLenEq].lengthEq - buferLenght;
                        //Удаление поставленной техники
                        if (oneBatt.listEquip[indMaxLenEq].numEq == 1) oneBatt.listEquip.RemoveAt(indMaxLenEq);
                        else
                        {
                            EquipInBatt dump = oneBatt.listEquip[indMaxLenEq];
                            dump.numEq--;
                            oneBatt.listEquip[indMaxLenEq] = dump;
                        }
                        //Добавляем технику и вагон в список
                        listEqs.Add(equip);
                        listWags.Add(wagon);
                        //Определям возможность установки техники на данный отрезок
                        maxLenEq = oneBatt.listEquip.Find(x => x.lengthEq <= freePlaceWagon).lengthEq;
                        if (maxLenEq != 0)
                        {
                            indMaxLenEq = oneBatt.listEquip.FindIndex(x => x.lengthEq == maxLenEq);
                            equip = new EquipInVesh()
                            {
                                codeEq = oneBatt.listEquip[indMaxLenEq].codeEq,
                                color = oneBatt.listEquip[indMaxLenEq].color,
                                lenFromBegPl = lenghtPlatform - freePlaceWagon,
                                lengthEq = oneBatt.listEquip[indMaxLenEq].lengthEq,
                                numWagon = wagon.numWagon
                            };
                            freePlaceWagon = freePlaceWagon - oneBatt.listEquip[indMaxLenEq].lengthEq - buferLenght;
                            //Удаление поставленной техники
                            if (oneBatt.listEquip[indMaxLenEq].numEq == 1) oneBatt.listEquip.RemoveAt(indMaxLenEq);
                            else
                            {
                                EquipInBatt dump = oneBatt.listEquip[indMaxLenEq];
                                dump.numEq--;
                                oneBatt.listEquip[indMaxLenEq] = dump;
                            }
                            listEqs.Add(equip);
                        }
                        //Определяем возможность установки на сцеп
                        if (freePlaceWagon >= 2000 && oneBatt.listEquip.Where(x => x.typeEq == true).Count() != 0 && flagUnistallScep != 1)
                        {
                            flagType = true;
                            if (flagUnistallScep == 2) LenEq = oneBatt.listEquip.Where(x => x.typeEq == flagType).Max(y => y.lengthEq);
                            else if (flagUnistallScep == 3) LenEq = oneBatt.listEquip.Where(x => x.typeEq == flagType).Min(y => y.lengthEq);
                            indMaxLenEq = oneBatt.listEquip.FindIndex(x => x.lengthEq == LenEq && x.typeEq == flagType);
                            //Определяем свободное место на платформе на следующем вагоне
                            freePlaceWagon = lenghtPlatform - buferLenght - (oneBatt.listEquip[indMaxLenEq].lengthEq - 2000 - lenghtScep);
                            //Определяем возможность установки на следующий вагон (целесообразность использования сцепа)
                            if (oneBatt.listEquip.Where(x => x.lengthEq >= freePlaceWagon).Count() != 0)
                            {
                                freePlaceWagon = lenghtPlatform;
                                continue;
                            }
                            equip = new EquipInVesh()
                            {
                                codeEq = oneBatt.listEquip[indMaxLenEq].codeEq,
                                color = oneBatt.listEquip[indMaxLenEq].color,
                                lenFromBegPl = lenghtPlatform - 2000,
                                lengthEq = oneBatt.listEquip[indMaxLenEq].lengthEq,
                                numWagon = wagon.numWagon
                            };
                            //Удаление поставленной техники
                            if (oneBatt.listEquip[indMaxLenEq].numEq == 1 && oneBatt.listEquip.Count == 1)
                            {
                                freePlaceWagon = lenghtPlatform;
                                continue;
                            }
                            else if (oneBatt.listEquip[indMaxLenEq].numEq == 1) oneBatt.listEquip.RemoveAt(indMaxLenEq);
                            else
                            {
                                EquipInBatt dump = oneBatt.listEquip[indMaxLenEq];
                                dump.numEq--;
                                oneBatt.listEquip[indMaxLenEq] = dump;
                            }
                            listEqs.Add(equip);
                        }
                        else freePlaceWagon = lenghtPlatform;
                    }

                    if (oneBatt.listEquip.Count == 0)
                    {
                        flagVesh = false;
                    }
                    
                    VoinEshelon vEshelon = new VoinEshelon()
                    {
                        nameVeSh = oneBatt.nameUnit,
                        numWagon = --ii,
                        massVeSh = 500,
                        listEquips = listEqs,
                        listWagons = listWags
                    };

                    CollEshelon.Add(vEshelon);
                }
            }
            return CollEshelon;
        }

        //Структура подразделения выбранного на погрузку
        public struct BuildnUnit
        {
            public int numUnit;         // порядковый номер подразделения
            public string nameUnit;     // наименование подразделения
            public string belongUnit;   // принадлежность подразделения 
            public int numOficer;       // число Офицеров
            public int numSerjant;      // число Сержантов
            public int numSoldat;       // число Солдат
            public int numGP;           // число Гражданских
                                        //Коллекция ВВТ в подразделении
            public List<EquipInBatt> listEquip;
            public BuildnUnit(int nU, string nmU, string bU, int nO, int nS, int nSol, int nG, List<EquipInBatt> lEq) //List<EquipInBatt> lEqs, List<EquipInBatt> lEqu)
            {
                numUnit = nU;
                nameUnit = nmU;
                belongUnit = bU;
                numOficer = nO;
                numSerjant = nS;
                numSoldat = nSol;
                numGP = nG;
                listEquip = lEq;
            }
        }

        //Структура ВВТ в выбранном батальоне на погрузку
        public struct EquipInBatt
        {
            public string codeEq;       // Шифр техники
            public Color color;         // Цвет техники
            public int lengthEq;        // Длина техники
            public int massEq;          // Масса техники
            public int numEq;           // Количество данной техники
            public bool typeEq;         // Тип техники
            public EquipInBatt(string cE, Color cl, int lE, int mE, int nE, bool tE)
            {
                codeEq = cE;
                color = cl;
                lengthEq = lE;
                massEq = mE;
                numEq = nE;
                typeEq = tE;
            }
        }

        //Структура эшелона
        public struct VoinEshelon
        {
            public string nameVeSh;     // наименование подразделения в эшелоне
            public int numWagon;       // число вагонов
            public int massVeSh;      // перевозимая масса
                                      //Коллекции ВВТ и вагонов в эшелоне
            public List<EquipInVesh> listEquips;
            public List<WagonInVesh> listWagons;
            public VoinEshelon(string nV, int nW, int mV, List<EquipInVesh> lEqs, List<WagonInVesh> lWags)
            {
                nameVeSh = nV;
                numWagon = nW;
                massVeSh = mV;
                listEquips = lEqs;
                listWagons = lWags;
            }
        }

        //Структура ВВТ в эшелоне
        public struct EquipInVesh
        {
            public string codeEq;       // Шифр техники
            public Color color;         // Цвет техники
            public int lengthEq;        // Длина техники
            public int numWagon;        // Номера платформы на которой стоят передние колеса
            public int lenFromBegPl;    // Расстояние от начала платформы
            public EquipInVesh(string cE, Color cl, int lE, int nW, int lFBP)
            {
                codeEq = cE;
                color = cl;
                lengthEq = lE;
                numWagon = nW;
                lenFromBegPl = lFBP;
            }
        }
        //Структура вагона в эшелоне
        public struct WagonInVesh
        {
            public int numWagon;       // Порядковый номер вагона
            public byte typeWagon;     // Тип вагона (1 - платформа, 2 - купе, 3 - плацкарт, 4 - кухня)
            public WagonInVesh(int nW, byte tW)
            {
                numWagon = nW;
                typeWagon = tW;
            }
        }

        public Color DefineColor(int rgb)
        {
            Color color = new Color();
            switch (rgb)
            {
                case 1:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                case 2:
                    color = Color.FromArgb(0, 255, 0);
                    break;
                case 3:
                    color = Color.FromArgb(0, 0, 255);
                    break;
                case 4:
                    color = Color.FromArgb(255, 255, 0);
                    break;
                case 5:
                    color = Color.FromArgb(255, 0, 255);
                    break;
                case 6:
                    color = Color.FromArgb(0, 255, 255);
                    break;
                case 7:
                    color = Color.FromArgb(255, 125, 0);
                    break;
                case 8:
                    color = Color.FromArgb(255, 0, 125);
                    break;
                case 9:
                    color = Color.FromArgb(255, 125, 125);
                    break;
                case 10:
                    color = Color.FromArgb(125, 255, 0);
                    break;
                case 11:
                    color = Color.FromArgb(0, 255, 125);
                    break;
                case 12:
                    color = Color.FromArgb(125, 255, 125);
                    break;
                case 13:
                    color = Color.FromArgb(125, 0, 255);
                    break;
                case 14:
                    color = Color.FromArgb(0, 125, 255);
                    break;
                case 15:
                    color = Color.FromArgb(125, 125, 255);
                    break;
                case 16:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                case 17:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                case 18:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                case 19:
                    color = Color.FromArgb(255, 0, 0);
                    break;
                default:
                    color = Color.Black;
                    break;
            }
            return color;
        }
    }
}