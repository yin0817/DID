///*
//  ###CSharp Code Generate###
//  Project
//  Create by User(EMAIL) 2022/9/1 17:50:53
//  项目表

//Project(项目表)
//---------------------------------------
//ProjectId(编号)         PKString(50)
//Name(名称)              Enum
//CreditScore(信用分)     Enum
//Mail(邮箱)              Enum
//Telegram(电报群)        Enum
//Country(国家)           Enum
//Province(省)            Enum
//City(市)                Enum
//Area(区)                Enum
//Uid(编号)               Enum
//RegDate(注册时间)       Enum

//*/

//using System;
//using System.Text;

//namespace Project
//{
//    public enum NameEnum { Unknown, Value1, Value2 }

//    public enum CreditScoreEnum { Unknown, Value1, Value2 }

//    public enum MailEnum { Unknown, Value1, Value2 }

//    public enum TelegramEnum { Unknown, Value1, Value2 }

//    public enum CountryEnum { Unknown, Value1, Value2 }

//    public enum ProvinceEnum { Unknown, Value1, Value2 }

//    public enum CityEnum { Unknown, Value1, Value2 }

//    public enum AreaEnum { Unknown, Value1, Value2 }

//    public enum UidEnum { Unknown, Value1, Value2 }

//    public enum RegDateEnum { Unknown, Value1, Value2 }

//    /* 项目表 */
//    public class Project
//    {
//        protected string projectId;
//        protected NameEnum name;
//        protected CreditScoreEnum creditScore;
//        protected MailEnum mail;
//        protected TelegramEnum telegram;
//        protected CountryEnum country;
//        protected ProvinceEnum province;
//        protected CityEnum city;
//        protected AreaEnum area;
//        protected UidEnum uid;
//        protected RegDateEnum regDate;

//        public Project()
//        {
//        }

//        //编号
//        public string ProjectId
//        {
//            get { return projectId; }
//            set { projectId = value; }
//        }
//        //名称
//        public NameEnum Name
//        {
//            get { return name; }
//            set { name = value; }
//        }
//        //信用分
//        public CreditScoreEnum CreditScore
//        {
//            get { return creditScore; }
//            set { creditScore = value; }
//        }
//        //邮箱
//        public MailEnum Mail
//        {
//            get { return mail; }
//            set { mail = value; }
//        }
//        //电报群
//        public TelegramEnum Telegram
//        {
//            get { return telegram; }
//            set { telegram = value; }
//        }
//        //国家
//        public CountryEnum Country
//        {
//            get { return country; }
//            set { country = value; }
//        }
//        //省
//        public ProvinceEnum Province
//        {
//            get { return province; }
//            set { province = value; }
//        }
//        //市
//        public CityEnum City
//        {
//            get { return city; }
//            set { city = value; }
//        }
//        //区
//        public AreaEnum Area
//        {
//            get { return area; }
//            set { area = value; }
//        }
//        //编号
//        public UidEnum Uid
//        {
//            get { return uid; }
//            set { uid = value; }
//        }
//        //注册时间
//        public RegDateEnum RegDate
//        {
//            get { return regDate; }
//            set { regDate = value; }
//        }
//        public void Reset()
//        {
//            projectId = null;
//            name = NameEnum.Unknown;
//            creditScore = CreditScoreEnum.Unknown;
//            mail = MailEnum.Unknown;
//            telegram = TelegramEnum.Unknown;
//            country = CountryEnum.Unknown;
//            province = ProvinceEnum.Unknown;
//            city = CityEnum.Unknown;
//            area = AreaEnum.Unknown;
//            uid = UidEnum.Unknown;
//            regDate = RegDateEnum.Unknown;
//        }

//        public void AssignFrom(Project AObj)
//        {
//            if (AObj == null)
//            {
//                Reset();
//                return;
//            }
//            projectId = AObj.projectId;
//            name = AObj.name;
//            creditScore = AObj.creditScore;
//            mail = AObj.mail;
//            telegram = AObj.telegram;
//            country = AObj.country;
//            province = AObj.province;
//            city = AObj.city;
//            area = AObj.area;
//            uid = AObj.uid;
//            regDate = AObj.regDate;
//        }

//    }

//}

