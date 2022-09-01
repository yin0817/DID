///*
//  ###CSharp Code Generate###
//  UserProject
//  Create by User(EMAIL) 2022/9/1 17:50:27
//  用户项目关系表

//UserProject(用户项目关系表)
//--------------------------------------------
//UserProject(编号)          PKString(50)
//DIDUserId(用户编号)        FKString(50) //<<关联:DIDUser.DIDUserId>>
//ProjectId(项目编号)        FKString(50) //<<关联:Project.ProjectId>>
//CreateDate(创建时间)       Date
//BindingDate(绑定时间)      Date
//UnboundDate(解绑时间)      Date
//IsBind(是否绑定)           Enum
//Name(名称)                 String(50)
//Remarks(备注)              String(255)
//IsDelete(是否删除)         Enum

//*/

//using System;
//using System.Text;

//namespace UserProject
//{
//    public enum IsBindEnum { Unknown, Value1, Value2 }

//    public enum IsDeleteEnum { Unknown, Value1, Value2 }

//    /* 用户项目关系表 */
//    public class UserProject
//    {
//        protected string userProject;
//        protected string dIDUserId;
//        protected string projectId;
//        protected DateTime createDate;
//        protected DateTime bindingDate;
//        protected DateTime unboundDate;
//        protected IsBindEnum isBind;
//        protected string name;
//        protected string remarks;
//        protected IsDeleteEnum isDelete;

//        public UserProject()
//        {
//        }

//        //编号
//        public string UserProject
//        {
//            get { return userProject; }
//            set { userProject = value; }
//        }
//        //用户编号
//        public string DIDUserId
//        {
//            get { return dIDUserId; }
//            set { dIDUserId = value; }
//        }
//        //项目编号
//        public string ProjectId
//        {
//            get { return projectId; }
//            set { projectId = value; }
//        }
//        //创建时间
//        public DateTime CreateDate
//        {
//            get { return createDate; }
//            set { createDate = value; }
//        }
//        //绑定时间
//        public DateTime BindingDate
//        {
//            get { return bindingDate; }
//            set { bindingDate = value; }
//        }
//        //解绑时间
//        public DateTime UnboundDate
//        {
//            get { return unboundDate; }
//            set { unboundDate = value; }
//        }
//        //是否绑定
//        public IsBindEnum IsBind
//        {
//            get { return isBind; }
//            set { isBind = value; }
//        }
//        //名称
//        public string Name
//        {
//            get { return name; }
//            set { name = value; }
//        }
//        //备注
//        public string Remarks
//        {
//            get { return remarks; }
//            set { remarks = value; }
//        }
//        //是否删除
//        public IsDeleteEnum IsDelete
//        {
//            get { return isDelete; }
//            set { isDelete = value; }
//        }
//        public void Reset()
//        {
//            userProject = null;
//            dIDUserId = null;
//            projectId = null;
//            createDate = 0;
//            bindingDate = 0;
//            unboundDate = 0;
//            isBind = IsBindEnum.Unknown;
//            name = null;
//            remarks = null;
//            isDelete = IsDeleteEnum.Unknown;
//        }

//        public void AssignFrom(UserProject AObj)
//        {
//            if (AObj == null)
//            {
//                Reset();
//                return;
//            }
//            userProject = AObj.userProject;
//            dIDUserId = AObj.dIDUserId;
//            projectId = AObj.projectId;
//            createDate = AObj.createDate;
//            bindingDate = AObj.bindingDate;
//            unboundDate = AObj.unboundDate;
//            isBind = AObj.isBind;
//            name = AObj.name;
//            remarks = AObj.remarks;
//            isDelete = AObj.isDelete;
//        }

//    }

//}

