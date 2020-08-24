app.constant('constants', {
   // apiURL: 'http://localhost:15536/',
   //membershipModuleApiURL: 'http://localhost:50275/',
    apiURL: 'http://192.168.0.30:9066/',
    membershipModuleApiURL: 'http://192.168.0.30:9067/',
   // SiteUrl:'http://192.168.0.30:9069/',
    //client QA
    // apiURL: 'http://103.195.7.84:9094',
    // membershipModuleApiURL: 'http://103.195.7.84:9092',
    // SiteUrl:'http://103.195.7.84:9091',
    SessionTimeOutTimeForMobile:1800,
    SessionTimeOutTimeForDesktop:1800,
    apiKey: 'z110b112g100t120n116c105i104j121',
    StoreId: 1,
    LanguageId: 1,
    CurrencyId: 1,
    customerGUID: "00000000-0000-0000-0000-000000000000",
    CategoryId_Grocer: 49,
    //Dev
   // CategoryId_Fresh: 113,
   // FreshProductId :223,
    //QA or Live
     CategoryId_Fresh: 69,
     FreshProductId :230,
    CategoryId_Gourment: 50,
    CategoryId_Promotions: 117,
    //Dev
    //Promotions_productId:1236,
    //QA or Live
    Promotions_productId:281,
    CategoryId_PackageType: 116,
    Grocer_CategoryName: "Grocer",
    Gourment_CategoryName: "Gourmet",
    Fresh_CategoryName: "Fresh",
    PackageType_CategoryName:"FreshDeposit",
    StoreClientType: "store",
    POSClientType:"POS",
    PageName:"LandingPage",
    Cancelation_IntialisationSatus:5,
    Allow_Numbers_Pattern: /^[0-9]*$/,
    Allow_AlphaNumeric_Pattern: /^[a-zA-Z0-9]+$/,
    HTTPPost: 'Post',
    HTTPGet: 'Get',
    signupApiURL: '/Api/Client/Register',
    FREE_SHIPPING_LIMIT: 'FREE_SHIPPING_LIMIT',
    MEMBERSHIP_PLANS: {'MONTHLY': 1, 'Loyalty': 6},
    DISCOUNTS: { 'DISCOUNT_GROCER': 'DISCOUNT_GROCER', 'DISCOUNT_GOURMET': 'DISCOUNT_GOURMET', 'DISCOUNT_FRESH': 'DISCOUNT_FRESH' },
    PAGE_NAMES: { 'CART': 'CART', 'CHECKOUT': 'CHECKOUT', 'NOREDIRECT': 'NOREDIRECT', 'FRESHCART': 'FRESHCART', 'FRESHSHIPPING': 'FRESHSHIPPING','POSCHECKOUT': 'POSCHECKOUT' },
    PRODUCT_LIST_ORDER_NUMBERS: { 'PRICE_ASC': 10, 'PRICE_DESC': 11 },
    EmailExist: -1,
    PhoneExist: -2,
    ProductListItemCount: 16,
    ProductListFilterNames: { 'Discount': 'Discount', 'Popularity': 'Popularity', 'Recent': 'Recent', 'Offer': 'Offer', 'SubCategory': 'SubCategory', 'Category': 'Category', 'TopSeller': 'TopSeller' },
    UserPurchaseStatusIds: { 'AddToCart': 1, 'Checkout': 2, 'Confirmed': 3, 'Shipped': 4, 'Delivered': 5 },
    PurchaseTypeIds: { 'COD': 1, 'Online': 2 },
    AdminRoleMask: '@Sam_@',
    Config_DeliveryCharges: 'DeliveryCharges',
    Config_DeliveryChargeMinLimit: 'DeliveryChargeMinLimit',
    DeliveryChargeMinLimit: 100,
    NumberofReviewstobeLoaded: 5,
    ReviewType: { 'Product': 'Product', 'Site': 'Site' },
	POSRoleId: 21,
    /*Shipping Names*/
    ShippingName_Free: 'FREE___Shipping.FixedByWeightByTotal',
    ShippingName_FreshDelivery: '5MEALDELIVERY___Shipping.FixedByWeightByTotal',
    /*Shipping Names*/

    Stripe_Publishable_Key: 'pk_test_As7b4Ukq3aYv3AxYZosl2zSw',
    POS_User_Redirect: ["product", "products", "productDetails", "commonCheckout", "freshMeal", "freshCart", "freshPlanCustomize", "freshPlanChoose", "freshThankyou", "freshLogistic", "freshOrderSummary", "freshShipping"],
    ValidationPatterns: {
        Alphabets: /^[a-zA-Z ]*$/, AlphaNumerics: /^[a-zA-Z 0-9]+$/, Numbers: /^[0-9]*$/, Decimals: /^(?=.*\d)\d*[\.]?\d*$/,
        AlphabetsWithoutSpaces: /^[a-zA-Z]*$/,
        NumbersGreaterThanZero: /^(1|[1-9][0-9]*)$/,
        PhoneNumber: /^(\+\d{1,3}[- ]?)?\d{10}$/,
        AlphaNumericsWithoutSpaces: /^[a-zA-Z 0-9]+$/,
        HttpURL: /^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/,
        Email: /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/,
        UserPassword: /^(?=.*[a-zA-Z])(?=.*\d)[A-Za-z\d][A-Za-z\d]{7,15}$/,
       //UserPassword: /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d][A-Za-z\d!@#$%^&*()_+]{8,15}$/,
        //UserPassword: /^[a-zA-Z0-9]{8,15}$/,
        AlphabetsPlusAmpersand: /^[a-zA-Z& ]*$/,
        AlphabetsPluscomma: /^[a-zA-Z, ]*$/
    },
    ValidationMessages: {
        Added: 'Added Successfully',
        Updated: 'Updated Successfully',
        Deleted: 'Deleted Successfully',
        Exists: 'Already Exists',
        Dependency: 'Dependency Exists',
        ConfirmDelete: 'Are you sure you want to Delete',
        Terms_Conditions: 'Check the Terms and Conditions to Continue',
        ShippingDetails: 'Select Atleast one shipping address to continue',
        CartDetails: 'Please Add atleast one Product to continue',
        UserEmailExists: 'Email Already Exist, Please try again',
        UserPhoneExists: 'Phone Number Already Exist, Please try again',
        OTPFromResponse: 'OTP has been sent to your number, please enter the OTP to confirm registration',
        OTPInvalid: 'Invalid OTP',
        UserPhoneDoesNotExists: 'Invalid Phone Number',
        ForgotPasswordSuccess: 'Password Reset Successfully and Sent to your Number',
        ForgotPasswordMailNotFound: 'Entered mail id is not registered with KIPOS COLLECTIVE'
    },

    /*Timings for Delivery and Pickup*/
    LeadTimeforGourmetPickUpCurrentDayMinutes: 30,
    LeadTimeforGourmetDeliveryCurrentDayHours:1,
    MinTimeForGrocerInt: 9,
    MaxTimeForGrocerInt: 21,
    MaxTimeForGourmetPickupHourInt: 19,
    MaxTimeForGourmetPickupMinutesIntMax: 31,
    MaxTimeForGourmetPickupWeekendHourInt: 18,
    MaxTimeForGourmetPickupWeekendMinutesIntMax: 31,
    MaxTimeForGourmetDeliveryHourInt: 20,
    MaxTimeForGourmetDeliveryMinuteInt: 0,
    MinTimeForGourmetDeliveryCurrentDayInt:19,
    MaxTimeForGourmetPickupMinutesIntMin: 30,
    MinTimeForGourmetDeliveryWeekendCurrentDayInt: 18,
    MaxTimeForGourmetDeliveryWeekendHourInt: 19,
    MaxTimeForGourmetDeliveryWeekendMinuteInt: 0,
    MinTimeForGrocer: "09:00",
    MinTimeForGourmetNotCurrentDay: "10:00",
    MinTimeForGourmetWeekendNotCurrentDay: "11:00",
    MinTimeForGourmetPickupNotCurrentDay: "10:00",
    MinTimeForGourmetWeekendPickupNotCurrentDay: "11:00",
    MinTimeForGourmetDeliveryNotCurrentDay: "10:00",
    MinTimeForGourmetWeekendDeliveryNotCurrentDay: "11:00",
    MaxTimeForGrocer: "21:00",
    MaxTimeForGourmetDelivery: "20:00",
    MaxTimeForGourmetWeekendDelivery: "19:00",
    MaxTimeForGourmetDeliveryCurrentDay:"19:00",
    MaxTimeForGourmetPickUpCurrentDayMax: "19:31",
    MaxTimeForGourmetPickUpCurrentDayMin: "19:30",
    MaxTimeForGourmetPickUpNotCurrentDay: "20:00",
    MaxTimeForGourmetPickUp: "20:00",
    MaxTimeForGourmetWeekendPickUp: "19:00",
    MaxTimeForGourmetPickUpWeekendCurrentDayMax: "18:31",
    MaxTimeForGourmetDeliveryWeekendCurrentDay: "18:00",
    // StoreOpeningHour: 9,
    // StoreOpeningMins: 0,
    // StoreCloseHour: 21,
    // StoreCloseMins: 0,
    // StrtOrdHrforGourmetPickUp: 8,
    // StrtOrdMinforGourmetPickUp: 30,
    // EndOrdHrforGourmetPickUp: 20,
    // EndOrdMinforGourmetPickUp: 30,

    //Store Timings Start
    // StoreOpeningTimeOnWeekDays:"10:00",
    // StoreClosingTimeOnWeekDays:"20:00",
    // StoreOpeningTimeOnWeekend:"11:00",
    // StoreClosingTimeOnWeekend:"19:00",
  
    //store timings end




    // updated by Phani for increase in price for ingredient > 1  on 11-09-2019 started
    ProductExeText: { 'BASE': 'BASE$1', 'GREEN': 'GREENS$1', 'PROTEIN': 'PROTEIN$1', 'DRESSING': 'DRESSING$1', 'LUX': 'LUX$2', 'CRUNCH': 'CRUNCH$1'  },
    //For BYOB Regular
    NoBaseBYOBRegular: 2777,
    NoDressBYOBRegular: 1495,
    BaseProductAttributeMappingIdForBYOBRegular: 1129,
    DressProductAttributeMappingIdBYOBRegular: 1132,
    //For BYOB Samll
    NoBaseBYOBSmall: 1154,
    NoDressBYOBSmall: 1159,
    BaseProductAttributeMappingIdBYOBSmall: 2829,   
    DressProductAttributeMappingIdBYOBSmall: 2827,
    //For DailyBowl
    NoBaseDailyBowl: 2825,
    NoDressDailyBowl: 2826,
    BaseProductAttributeMappingIdDailyBowl: 1176,
    DressProductAttributeMappingIdDailyBowl: 1177,
    // updated by Phani for increase in price for ingredient > 1  on 11-09-2019 ended
    BYOBRegularBowlID:1233,

    BYOBSmallBowlID:1254,

    BYOBDailyBowlID:1256,

    BYOBName: "BUILD YOUR OWN BOWL (Regular)",

    BYOBName1: "BUILD YOUR OWN BOWL (Small)",

    DailyBowl: "THE DAILY BOWL",

    StripeMinimumAmount:"StripeMinimumAmount",

    globalCountryCodes: [
        //{ 'Id': 1, 'Code': '+61', 'Value': 'AUS +61', 'maxlength': '10' },
        //{ 'Id': 2, 'Code': '+91', 'Value': 'IND +91', 'maxlength': '10' },
        { 'Id': 3, 'Code': '+65', 'Value': 'SIN +65', 'maxlength': '8' }]
});