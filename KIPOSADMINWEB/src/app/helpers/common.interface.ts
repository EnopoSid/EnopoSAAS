export interface IPageLevelPermissions {
    Add:boolean,
    Edit: boolean,
    Delete: boolean,
    View: boolean,
}

export interface MenuModel{
        CreatedBy: number,
        CreatedDate: string,
        MenuId: number
        MenuColor: String,
        MenuIcon: number,
        MenuName:string,
        MenuURL: String,
        ModifiedBy: number,
        ModifiedDate: String,
        sno: number,
        IsActive: number
}

export interface RoleModel{
    sno: number,
    RoleId: number
    RoleName:string,
    CreatedBy: number,
    ModifiedBy: number,
    CreatedDate: string,
    ModifiedDate: String,
    IsActive: number
}

export interface RolePermissionModel{
    sno: number,
    RolePermissionId: number
    RolePermissionName:string,
    CreatedBy: number,
    ModifiedBy: number,
    CreatedDate: string,
    ModifiedDate: String,
    IsActive: number
}
export interface RegionModel{
    sno: number,
    RegionId:number,
    ZoneId:number,
    RegionName:string,
    CreatedBy:number,
    ModifiedBy:number,
    CreatedDate: string,
    ModifiedDate: String,
    IsActive: number
}
export interface CompalintTypeModel{
    sno: number,
    CompalintTypeId:number,
    CompalintTypeName:string,
    CreatedBy:number,
    ModifiedBy:number,
    CreatedDate: string,
    ModifiedDate: String,
    IsActive: number
}
export interface ZonenModel{
    sno: number,
    RegionId:number,
    ZoneId:number,
    ZoneName:string,
    RegionName:string,
    CreatedBy:number,
    ModifiedBy:number,
    CreatedDate: string,
    ModifiedDate: String,
    IsActive: number
}
export interface ServiceCategoryModel{
    sno: number,
    ServiceCategoryId:number,
    ServiceCategoryName:string,
    CreatedBy:number,
    ModifiedBy:number,
    CreatedDate: string,
    ModifiedDate: String,
    IsActive: number
}
export interface ComplaintModel {
    ID: string;
    ComplaintId: number;
    complaintNum : string;
    UserId: number;
     sno: number;
    ComplaintTypeId: number;
    ComplaintDetails: string;
    IsReportedToServiceProvider:boolean;
    ServiceProviderReportDetails: string;
    ServiceProviderComplaintRefNum: string;
    CustomerCareAgentName: string;
    HaveContactedWithusBefore: boolean;
    PreviousContactedDetails: string;
    CommunicationId: number;
    ComplaintReceivedFrom: number;
    ComplaintStatusId: number;
    ServiceCategoryId: number;
    ZoneId: number;
    ServiceProviderId: number;
    CreatedDate : string;
    ModifiedDate: string;
    CreatedBy: number;
    ModifiedBy: number;
    IsActive: boolean;
    ServiceProviderNameIfOthers: string;
    ServiceCategoryNameIfOthers: string;
    ComplaintTypeNameIfOthers: string;
    canEdit : boolean;
}

export class EnquiryModel{
    ModifiedBy:number
    CreatedBy:number
    IpAddress : string
    EnquiryDetails:string
    IsActive:number
}

export interface UserModel {
    ID: string;
    id :number;
   UserName:string,
    Password: string;
    FirstName: string;
    LastName: string;
    sno: number;
    CreatedBy: number;
    CreatedDate: string;
    Department: number;
    Email: string;
    modifiedBy: number;
    modifiedDate: String;
    phone: number;
    privilegeId: number;
    statusId: number;
    userID: number;
    groupDutyRoster: number;
     gender:number;
     roleType:number;
     POSId:number;
}

export interface ConfigurationModel{
    ModifiedBy: number,
    CreatedBy: number,
    KeyName:string,
    keyValue:number,
    ConfigurationId:number,
    IsActive:number;
}
export interface SubMenuModel{
    SubMenuName: string,
    SubMenuUrl:string,
    MenuId:number,
    IsActive:number,
    CreatedBy: number,
    ModifiedBy: number;  
}
export interface ServiceProviderModel{
    ServiceProviderName: string
    IsActive: number,
    CreatedBy: number,
    ModifiedBy: number,
}
export interface ComplaintStatusModel{
    ComplaintStatusName: string,
    IsActive: number,
    CreatedBy:number,
    ModifiedBy: number,
}
export interface CommunicationStatusModel{
    ModifiedBy: number,
    CreatedBy: number,
    CommunicationName:string,
    CommunicationId: number,
    IsActive:number,
}
export interface DepartmentModel{
    ModifiedBy: number,
    CreatedBy: number,
    DepartmentName:string,
    DepartmentId: number,
    IsActive:number,
}
export interface ComplaintReportModel{
    complaintType:  string,
    complaintStatusId:number,
    fromdate: Date,
    toDate : Date,
    region: string,
    serviceCategory:string,
    ComplaintStatusName:string,
}
export interface OperatorAndStaffReportModel{
    complaintType:  string,
    complaintStatusId:number,
    fromdate: Date,
    toDate : Date,
    region: string,
    serviceCategory:string,
}
export interface EnquiryReportModel{
    complaintType:  string,
    complaintStatusId:number,
    fromdate: Date,
    toDate : Date,
    region: string,
    serviceCategory:string,
}

export interface CtiModel{
    sno:number,
    PhoneNumber:number,
    ReceivedPhoneNumber:number
}
export interface StoresModel{
    sno: number,
    Name:string,
    CreatedBy: number,
    ModifiedBy: number,
    CreatedDate: string,
    ModifiedDate: String,
    IsActive: number
   
}
export interface PaymentModel{
    sno: number,
    Name:string,
    Id:string,
    CreatedBy: number,
    ModifiedBy: number,
    CreatedDate: string,
    ModifiedDate: String,
    IsActive: number
   
}

export interface reportSearchBy { 
    FromDate: Date,
    ToDate: Date,
    Category:string,
    orderstatus: string,
    issearchfilter : boolean
};

export interface OurStoryModel {
    Id: number;
    brandid:number,
    TitleName: string,
    SubTitleName:string,
    ShortDescription: string;
    FullDescription: string;
    ModifiedDate: string;
    sno: number;
    CreatedDate: string;
    ModifiedBy: number;
    CreatedBy: string,
    IsActive: number
}

export interface AboutUsModel {
    id: number;
    TitleName: string,
    ShortDescription: string;
    FullDescription: string;
    ModifiedDate: string;
    sno: number;
    CreatedDate: string;
    ModifiedBy: number;
    CreatedBy: string,
    IsActive: number
}

export interface FeatureArticleModel {

    ArticleTitleName: string,
    ArticlePostedBy: string;
    ArticleDescription: string;
    sno: number;
    image: string,
    IsActive: number

}

export interface NewsLetterModel {

    NewsLetter: string,
    Title:string
    CreatedDate: string;
    LastsendDate: string;
    sno: number;
    IsActive: number

}
export interface BannerImageModel{
    id: number;
    TitleName: string,
    ShortDescription: string;
    FullDescription: string;
    ModifiedDate: string;
    sno: number;
    CreatedDate: string;
    ModifiedBy: number;
    CreatedBy: string,
    IsActive: number

}
export interface InstagramModel {
    ModifiedBy: number,
    CreatedBy: number,
    KeyName: string,
    keyValue: number,
    ConfigurationId: number,
    IsActive: number;
    ImageId:string,
    ulrpath:string,   
    orderid:number
}