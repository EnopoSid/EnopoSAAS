export default class MyAppHttp {

    public static REQUESTS = {
        INITIALIZE: {
            modelId: 101,
            name: 'INITIALIZE',
            idletimeoutrestart: false,
            online: true,
            serviceType: 0,
            url: ''
        },
        HMAC: {
            modelId: 102,
            name: 'HMAC',
            idletimeoutrestart: false,
            online: true,
            serviceType: 1,
            url: ''
        },
        APPINFO: {
            modelId: 110,
            name: 'APPINFO',
            idletimeoutrestart: false,
            online: true,
            serviceType: 2,
            url: ''
        },
        LOGIN: {
            modelId: 150,
            name: 'LOGIN',
            idletimeoutrestart: false,
            online: true,
            serviceType: 2,
            url: ''
        },
        ADMIN_LOGIN: {
            modelId: 420,
            name: 'ADMIN_LOGIN',
            idletimeoutrestart: false,
            online: true,
            serviceType: 2,
            url: ''
        },
        LOGOUT: {
            modelId: 421,
            name: 'LOGOUT',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        INTERVIEWS: {
            modelId: 151,
            name: 'INTERVIEWS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        JOBSEEKERS: {
            modelId: 251,
            name: 'JOBSEEKERS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        INTERVIEWERS: {
            modelId: 351,
            name: 'INTERVIEWERS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        GETUSERS: {
            modelId: 440,
            name: 'GETUSERS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        DELETEUSER: {
            modelId: 451,
            name: 'DELETEUSER',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        GETBANKS: {
            modelId: 551,
            name: 'GETBANKS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        DELETEBANK: {
            modelId: 751,
            name: 'DELETEBANK',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        ADDBANK: {
            modelId: 651,
            name: 'ADDBANK',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        EDITBANK: {
            modelId: 851,
            name: 'EDITBANK',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        GETTECHNOLOGIES: {
            modelId: 951,
            name: 'TECHONOLOGIES',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        DELETETECHNOLOGY: {
            modelId: 1051,
            name: 'DELETETECHNOLOGY',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        EDITTECHNOLOGY: {
            modelId: 1151,
            name: 'EDITTECHNOLOGY',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        ADDTECHNOLOGY: {
            modelId: 1251,
            name: 'ADDTECHNOLOGY',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        GETRATINGS: {
            modelId: 1551,
            name: 'RATINGS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        DELETERATING: {
            modelId: 1851,
            name: 'RATINGS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        ADDRATING: {
            modelId: 1651,
            name: 'RATINGS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        EDITRATING: {
            modelId: 1751,
            name: 'RATINGS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        GETINTERVIEWLEVELS: {
            modelId: 1351,
            name: 'INTERVIEW_LEVELS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        ADD_INTERVIEW_LEVEL: {
            modelId: 701,
            name: 'ADD_INTERVIEW_LEVEL',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        UPDATE_INTERVIEW_LEVEL: {
            modelId: 801,
            name: 'UPDATE_INTERVIEW_LEVEL',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        DELETE_INTERVIEW_LEVEL: {
            modelId: 901,
            name: 'DELETE_INTERVIEW_LEVEL',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        GETPRICINGLEVELS: {
            modelId: 1451,
            name: 'PRICING_LEVELS',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        ADD_PRICING_LEVEL: {
            modelId: 1001,
            name: 'ADD_PRICING_LEVEL',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        UPDATE_PRICING_LEVEL: {
            modelId: 1101,
            name: 'UPDATE_PRICING_LEVEL',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        DELETE_PRICING_LEVEL: {
            modelId: 1201,
            name: 'DELETE_PRICING_LEVEL',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        CHANGE_PASSWORD: {
            modelId: 301,
            name: 'CHANGE_PASSWORD',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        FORGOT_PASSWORD: {
            modelId: 401,
            name: 'FORGOT_PASSWORD',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        JOBSEEKER_INTERVIEW_SCHEDULE: {
            modelId: 501,
            name: 'JOBSEEKER_INTERVIEW_SCHEDULE',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        INTERVIEWER_INTERVIEW_SCHEDULE: {
            modelId: 601,
            name: 'INTERVIEWER_INTERVIEW_SCHEDULE',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        },
        COMPLETE_INTERVIEW: {
            modelId: 200,
            name: 'COMPLETE_INTERVIEW',
            online: true,
            idletimeoutrestart: false,
            url: '',
            serviceType: 2
        }
    };

    public static RESPONSE_CODES = {
        FUNCTIONAL_ERROR: 91,
        TECHNICAL_ERROR: 92,
        SECURITY_ERROR: 93,
        SUCCESS: 0
    };

    public static REQUEST_TYPES = {
        ADD: "Add",
        UPDATE: "Update",
        DELETE: "Delete",
        CANCEL:"Cancel"
    };

    public static  REQUEST_TYPE_Reset = {
       RESETPASSWORD:"Resetpassword"
    };
    public static Conerted_REQUEST_TYPES = {
        MemToNonMem: "MemToNonMem",
        MemToFreeMem: "MemToFreeMem",
        NonMemToMem: "NonMemToMem",
        NonMemToFreeMem: "NonMemToFreeMem",
        FreeMemToMem: "FreeMemToMem",
        FreeMemToNonMem: "FreeMemToNonMem",

    };

    public static HTML_VALIDATION_PATTERNS ={
        "onlyNumbers": /^[0-9]*$/,
        "onlyAlphabets":/^[a-zA-Z ]*$/,
        "onlyAlphabetsCapsOnly":/^[A-Z ]*$/,
        "onlyAlphabetsWithoutSpaces": /^[a-zA-Z]*$/,
        "alphaNumerics": /^[a-zA-Z0-9 ]+$/,
        "floatNumbers": /[^\d.]/,
        "onlyNumbersStartswithZero": /^(1|[1-9][0-9]*)$/,
        "alphaNumericsWithoutSpaces": /^[a-zA-Z0-9]+$/,
        "alphaNumericsCapsOnly": /^[A-Z0-9]+$/,
        "phoneNumber":/^[0-9\+\-\ ]+$/,
        "alphanumericWithHyphen":/^[0-9a-zA-Z\- \/_?:.,\s]+$/,
        "emailidpattern":/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/,
        "Domain" :/^[a-zA-Z.,]+(\s{0,1}[a-zA-Z., ])*$/,
        "singapore_MobileNum": /\d{8}/,
   }
   public static MASTER_MENU_SUBMENUS =["menus", "submenus",'users',"roles","rolepermissions", "configurations","plans","stores","posusers","paymentoptions","resetpassword","storetimings"];

   public static MENUS_WITHOUT_SUBMENUS = ['complaints', 'enquiries', 'ctidevice'];

   public static MEMBER_MENU_SUBMENUS=["members","nonmembers","membersubscription","schedulerDetails","freeMembership","planConversion","kpoints","corpdomain"]

   public static ORDERS_MENU_SUBMENUS=["orders","cancelorder","posOrders","pendingorder","completedorder"]

   public static ViewProfileRouteUrl = 'viewprofile';

   public static ChangepasswordRouteUrl = 'change_password';

   public static ANONYMOUS_ROUTES = ["/", "/consumer_complaint/add", "/consumer_complaint/checkstatus", "/consumer_enquiry/add"]

   public static DEPENDANT_SUBMENUS = ["submenus", "rolepermissions","orderdetails"];

   public static POSTargets_SUBMENU_NAME = ["postargets","bowlstarget"];

   public static MASTER_MENU_NAME = 'master';

   public static MEMBER_MENU_NAME = 'members';

   public static ORDERS_MENU_NAME = 'orders';

   public static CMS_MENU_NAME = 'anonymousemail';

   public static REPORT_MENU_NAME = 'orderreport';

   public static DELIVERY_MENU_NAME = 'delivery';

   public static PosTargets_Menu_Name = 'postargets';

   public static Delivey_SUB_MENU_Name = ["deliveryorders"];

   public static REPORT_SUBMENU_NAME = ["orderreport","salesreport","orderdetails","allreport","IngredientCountReport","allorders"];

   public static CMS_SUBMENU_NAME = ['anonymousemail','sitereviews','careers','contactus','vendorenquiry', 'landingpage','categories'];

   public static DASHBOARD_MENU_NAME = 'dashboard';

   public static DASHBOARD_SUBMENU_NAMES = ['dashboard'];

   public static ROUTER_LOGIN = 'sessions/signin';

   public static ROLEIDS_WITH_DEPARTMENTS = [6,7];
   public static DEPARTMENT_OFFICER_ROLEID = 6;
   public static DEPARTMENT_MANAGER_ROLEID = 7;
   public static EXECUTIVE_ROLEID = 8;
   public static DESK_SUPERVISOR_ROLEID=4;
   public static DEPARTMENTID_OTHERTHAN_DEPARTMENTROLES = 2;
   public static ACTIVESTATUS = 1;
   public static INACTIVESTATUS = 0;
   public static ROLEIDS_WITHOUT_OPENCOMPLAINTS=[6,8];
   public static Operator = [3,4,5];
   public static CanAnswerCall = [3,4];
   public static Staff = [2,6,7,8];
   public static LIMIT = 10;
   public static PAGE_LIMIT_OPTIONS = [
       {'value':10},
       {'value':15},
       {'value':20},
       {'value':25},
       {'value':30},
       {'value':50},
       {'value':75},
       {'value':100}];

     //  For Dev
   // public static PARENTCATEGORIESDATA = [
       // {'CategoryId': 50, 'CategoryName': 'Gourmet'},
       // {'CategoryId': 49, 'CategoryName': 'Grocer'},
       // {'CategoryId': 113, 'CategoryName': 'Fresh'}]

    // public static OrderDetailsRoute="Order details"

    // public static PromotionsProductId=1236

   //For QA or Live
        public static PARENTCATEGORIESDATA = [
         {'CategoryId': 50, 'CategoryName': 'Gourmet'},
        {'CategoryId': 49, 'CategoryName': 'Grocer'},
         {'CategoryId': 69, 'CategoryName': 'Fresh'}]

   public static OrderDetailsRoute="ORDER DETAILS"

   public static PromotionsProductId=281

    public static SEARCHCOMPLAINTSBYRADIOBUTTONDATA = [
        {'id': 1, 'name': 'My Tasks', 'checked': true},
        {'id': 2, 'name': 'Open Complaints', 'checked': false},
        {'id': 3, 'name': 'Search Complaints', 'checked': false},
        {'id': 4, 'name': 'Incoming Call Complaints', 'checked': false}]

   public static PhoneNumberCountryCode = "+91-"

    public static COUNTRYCODE="91"

   public static chartnames = [
   {'id':1,'chartname':'CategoryPieChart','title':'Category' },
   {'id':2,'chartname':'DeliveryOrderBarChart', 'title':'DeliveryOrders'},
   {'id':3,'chartname':'PickUpOrderBarChart', 'title':'PickUpOrders'},
   {'id':4,'chartname':'providerBarChart', 'title':'provider Count'},
   {'id':5,'chartname':'categoryBarChart','title':'category Count' },
   {'id':6,'chartname':'CallDetailsPiechart', 'title':'call Details'}]
   public static DURATION = [
                                {'id':1, 'name':'0-5 days','fromvalue':5,'tovalue':0},
                                {'id':2, 'name':'6-10 days','fromvalue':10,'tovalue':6},
                                {'id':3, 'name':'11-15 days','fromvalue':15,'tovalue':11},
                                {'id':4, 'name':'16-25 days','fromvalue':25,'tovalue':16},
                                {'id':5, 'name':'26-45 days','fromvalue':45,'tovalue':26},
                                {'id':6, 'name':'45+ days','fromvalue':45,'tovalue':45}
                            ]
   public static StatusColorCode =[
                                    {'id':1, 'statusName':'Dinner','colour':'#FF0F00'},
                                    {'id':2, 'statusName':'Lunch','colour':'#FF6600'},
                                    {'id':3, 'statusName':'Pending','colour':'#FF9E01'},
                                    {'id':4, 'statusName':'Resolved','colour':'#FCD202'},
                                    {'id':5, 'statusName':'Deferred','colour':'#F8FF01'},
                                    {'id':6, 'statusName':'Re-Opened','colour':'#B0DE09'},
                                    {'id':7,'statusName':'Re-Forwarded','colour':'#04D215'},
                                    {'id':8,'statusName':'Cancelled','colour':'#0D8ECF'},
                                    {'id':9,'statusName':'Closed','colour':'#0D52D1'}]

    public static Months=[
        {'ID':1,'MONTHNAME':'JANUARY'},
        {'ID':2,'MONTHNAME':'FEBRUARY'},
        {'ID':3,'MONTHNAME':'MARCH'},
        {'ID':4,'MONTHNAME':'APRIL'},
        {'ID':5,'MONTHNAME':'MAY'},
        {'ID':6,'MONTHNAME':'JUNE'},
        {'ID':7,'MONTHNAME':'JULY'},
        {'ID':8,'MONTHNAME':'AUGUST'},
        {'ID':9,'MONTHNAME':'SEPTEMBER'},
        {'ID':10,'MONTHNAME':'OCTOBER'},
        {'ID':11,'MONTHNAME':'NOVEMBER'},
        {'ID':12,'MONTHNAME':'DECEMBER'}
    ]
    public static CallTypes = [
        {'id':'1','type':'InComing'},
        {'id':'2','type':'OutGoing'}
    ]
    public static Admin_ClientType="Admin";
    public static getObject(key, id): any {
        let arrayOfKeys = Object.keys(MyAppHttp.REQUESTS);
        let lobjX;
        arrayOfKeys.forEach(element => {
            let lobjObj = MyAppHttp.REQUESTS[element];
            if (String(lobjObj[key]) == String(id)) {
                lobjX = lobjObj;
                return;
            }
        });

        return lobjX;
    }

    public static apiKey="z110b112g100t120n116c105i104j121"
    public static StoreId="1"
    public static LanguageId="1"
    public static CurrencyId="1"
    public static customerGUID="1"

}
