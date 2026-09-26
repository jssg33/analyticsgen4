using Enterprise.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
//using weathertest;

namespace Enterpriseservices
{
    // 01 Park
    /*public interface IParkService01
    {
        IEnumerable<Park> GetAll01();
        IEnumerable<Park> GetById01(int id);
        Task<IResult> Create01(Park input);
        Task<IResult> Update01(int id, Park input);
        Task<IResult> Delete01(int id);
    }*/

    // 02 Apilog
    public interface IApilogService02
    {
        IEnumerable<Apilog> GetAll02();
        IEnumerable<Apilog> GetById02(int id);
        Task<IResult> Create02(Apilog input);
        Task<IResult> Update02(int id, Apilog input);
        Task<IResult> Delete02(int id);
    }

    /*
    // 03 Template
    public interface ITemplateService03
    {
        IEnumerable<Template> GetAll03();
        IEnumerable<Template> GetById03(int id);
        Task<IResult> Create03(Template input);
        Task<IResult> Update03(int id, Template input);
        Task<IResult> Delete03(int id);
    }*/

    // 04 Userlog
    public interface IUserlogService04
    {
        IEnumerable<Userlog> GetAll04();
        IEnumerable<Userlog> GetById04(int id);
        Task<IResult> Create04(Userlog input);
        Task<IResult> Update04(int id, Userlog input);
        Task<IResult> Delete04(int id);
    }

    /*
    // 05 WeatherForecast
    public interface IWeatherForecastService05
    {
        IEnumerable<WeatherForecast> GetForecast05();
    }*/

    // 06 AdminLogs
    public interface IAdminLogsService06
    {
        IEnumerable<Adminlog> GetAll06();
        IEnumerable<Adminlog> GetById06(int id);
        Task<IResult> Create06(Adminlog input);
        Task<IResult> Update06(int id, Adminlog input);
        Task<IResult> Delete06(int id);
    }
/*
    // 07 Auth
    public interface IAuthService07
    {
        IEnumerable<Auth> GetAll07();
        IEnumerable<Auth> GetById07(int id);
        Task<IResult> Create07(Auth input);
        Task<IResult> Update07(int id, Auth input);
        Task<IResult> Delete07(int id);
    }

    // 08 Batch
    public interface IBatchService08
    {
        IEnumerable<Batch> GetAll08();
        IEnumerable<Batch> GetById08(int id);
        Task<IResult> Create08(Batch input);
        Task<IResult> Update08(int id, Batch input);
        Task<IResult> Delete08(int id);
    }

    // 09 Batchtype
    public interface IBatchtypeService09
    {
        IEnumerable<Batchtype> GetAll09();
        IEnumerable<Batchtype> GetById09(int id);
        Task<IResult> Create09(Batchtype input);
        Task<IResult> Update09(int id, Batchtype input);
        Task<IResult> Delete09(int id);
    }

    // 10 Booking
    public interface IBookingService10
    {
        IEnumerable<Booking> GetAll10();
        IEnumerable<Booking> GetById10(int id);
        Task<IResult> Create10(Booking input);
        Task<IResult> Update10(int id, Booking input);
        Task<IResult> Delete10(int id);
    }

    // 11 Cards
    public interface ICardsService11
    {
        IEnumerable<Card> GetAll11();
        IEnumerable<Card> GetById11(int id);
        Task<IResult> Create11(Card input);
        Task<IResult> Update11(int id, Card input);
        Task<IResult> Delete11(int id);
    }

    // 12 Cart
    public interface ICartService12
    {
        IEnumerable<Cart> GetAll12();
        IEnumerable<Cart> GetById12(int id);
        Task<IResult> Create12(Cart input);
        Task<IResult> Update12(int id, Cart input);
        Task<IResult> Delete12(int id);
    }

    // 13 CartItem
    public interface ICartItemService13
    {
        IEnumerable<Cartitem> GetAll13();
        IEnumerable<Cartitem> GetById13(int id);
        Task<IResult> Create13(Cartitem input);
        Task<IResult> Update13(int id, Cartitem input);
        Task<IResult> Delete13(int id);
    }

    // 14 CartMaster
    public interface ICartMasterService14
    {
        IEnumerable<CartMaster> GetAll14();
        IEnumerable<CartMaster> GetById14(int id);
        Task<IResult> Create14(CartMaster input);
        Task<IResult> Update14(int id, CartMaster input);
        Task<IResult> Delete14(int id);
    }

    // 15 Company
    public interface ICompanyService15
    {
        IEnumerable<Company> GetAll15();
        IEnumerable<Company> GetById15(int id);
        Task<IResult> Create15(Company input);
        Task<IResult> Update15(int id, Company input);
        Task<IResult> Delete15(int id);
    }

    // 16 Customer
    public interface ICustomerService16
    {
        IEnumerable<Customer> GetAll16();
        IEnumerable<Customer> GetById16(int id);
        Task<IResult> Create16(Customer input);
        Task<IResult> Update16(int id, Customer input);
        Task<IResult> Delete16(int id);
    }
/*
    // 17 Emailgateway
    public interface IEmailgatewayService17
    {
        IEnumerable<Emailgateway> GetAll17();
        IEnumerable<Emailgateway> GetById17(int id);
        Task<IResult> Create17(Emailgateway input);
        Task<IResult> Update17(int id, Emailgateway input);
        Task<IResult> Delete17(int id);
    }

    // 18 Employee
    public interface IEmployeeService18
    {
        IEnumerable<Employee> GetAll18();
        IEnumerable<Employee> GetById18(int id);
        Task<IResult> Create18(Employee input);
        Task<IResult> Update18(int id, Employee input);
        Task<IResult> Delete18(int id);
    }

    // 19 File
    public interface IFileService19
    {
        IEnumerable<File> GetAll19();
        IEnumerable<File> GetById19(int id);
        Task<IResult> Create19(File input);
        Task<IResult> Update19(int id, File input);
        Task<IResult> Delete19(int id);
    }

    // 20 LearnDetail
    public interface ILearnDetailService20
    {
        IEnumerable<Learnlog> GetAll20();
        IEnumerable<Learnlog> GetById20(int id);
        Task<IResult> Create20(Learnlog input);
        Task<IResult> Update20(int id, Learnlog input);
        Task<IResult> Delete20(int id);
    }

    // 21 ParkCalendar
    public interface IParkCalendarService21
    {
        IEnumerable<ParkCalendar> GetAll21();
        IEnumerable<ParkCalendar> GetById21(int id);
        Task<IResult> Create21(ParkCalendar input);
        Task<IResult> Update21(int id, ParkCalendar input);
        Task<IResult> Delete21(int id);
    }

    // 22 Parkreviews
    public interface IParkreviewService22
    {
        IEnumerable<ParkReview> GetAll22();
        IEnumerable<ParkReview> GetById22(int id);
        Task<IResult> Create22(ParkReview input);
        Task<IResult> Update22(int id, ParkReview input);
        Task<IResult> Delete22(int id);
    }

    // 23 Parks
    public interface IParkService23
    {
        IEnumerable<Park> GetAll23();
        IEnumerable<Park> GetById23(int id);
        Task<IResult> Create23(Park input);
        Task<IResult> Update23(int id, Park input);
        Task<IResult> Delete23(int id);
    }

    // 24 Payments
    public interface IPaymentService24
    {
        IEnumerable<Payment> GetAll24();
        IEnumerable<Payment> GetById24(int id);
        Task<IResult> Create24(Payment input);
        Task<IResult> Update24(int id, Payment input);
        Task<IResult> Delete24(int id);
    }

    // 25 Refunds
    public interface IRefundService25
    {
        IEnumerable<Refund> GetAll25();
        IEnumerable<Refund> GetById25(int id);
        Task<IResult> Create25(Refund input);
        Task<IResult> Update25(int id, Refund input);
        Task<IResult> Delete25(int id);
    }

    // 26 SalesCatalogue
    public interface ISalesCatalogueService26
    {
        IEnumerable<SalesCatalogue> GetAll26();
        IEnumerable<SalesCatalogue> GetById26(int id);
        Task<IResult> Create26(SalesCatalogue input);
        Task<IResult> Update26(int id, SalesCatalogue input);
        Task<IResult> Delete26(int id);
    }

   // 27 Sessionlog
    public interface ISessionlogService27
    {
        IEnumerable<Sessionlog> GetAll27();
        IEnumerable<Sessionlog> GetById27(int id);
        Task<IResult> Create27(Sessionlog input);
        Task<IResult> Update27(int id, Sessionlog input);
        Task<IResult> Delete27(int id);
    }

    // 28 Site
    public interface ISiteService28
    {
        IEnumerable<Site> GetAll28();
        IEnumerable<Site> GetById28(int id);
        Task<IResult> Create28(Site input);
        Task<IResult> Update28(int id, Site input);
        Task<IResult> Delete28(int id);
    }

    // 29 SuperuserLogs
    public interface ISuperuserLogService29
    {
        IEnumerable<Superuserlog> GetAll29();
        IEnumerable<Superuserlog> GetById29(int id);
        Task<IResult> Create29(Superuserlog input);
        Task<IResult> Update29(int id, Superuserlog input);
        Task<IResult> Delete29(int id);
    }

    // 30 TaxtableState
    public interface ITaxtableStateService30
    {
        IEnumerable<TaxtableState> GetAll30();
        IEnumerable<TaxtableState> GetById30(int id);
        Task<IResult> Create30(TaxtableState input);
        Task<IResult> Update30(int id, TaxtableState input);
        Task<IResult> Delete30(int id);
    }

    // 31 TaxtableUS
    public interface ITaxtableUSService31
    {
        IEnumerable<TaxtableU> GetAll31();
        IEnumerable<TaxtableU> GetById31(int id);
        Task<IResult> Create31(TaxtableU input);
        Task<IResult> Update31(int id, TaxtableU input);
        Task<IResult> Delete31(int id);
    }

    // 32 Techs
    public interface INoctechService32
    {
        IEnumerable<Noctech> GetAll32();
        IEnumerable<Noctech> GetById32(int id);
        Task<IResult> Create32(Noctech input);
        Task<IResult> Update32(int id, Noctech input);
        Task<IResult> Delete32(int id);
    }

    // 33 TestBooking
    public interface IBookingService33
    {
        IEnumerable<Booking> GetAll33();
        IEnumerable<Booking> GetById33(int id);
        Task<IResult> Create33(Booking input);
        Task<IResult> Update33(int id, Booking input);
        Task<IResult> Delete33(int id);
    }

    // 34 User
    public interface IUserService34
    {
        IEnumerable<User> GetAll34();
        IEnumerable<User> GetById34(int id);
        Task<IResult> Create34(User input);
        Task<IResult> Update34(int id, User input);
        Task<IResult> Delete34(int id);
    }

    // 35 Useraction
    public interface IUseractionService35
    {
        IEnumerable<Useraction> GetAll35();
        IEnumerable<Useraction> GetById35(int id);
        Task<IResult> Create35(Useraction input);
        Task<IResult> Update35(int id, Useraction input);
        Task<IResult> Delete35(int id);
    }

    // 36 Usergroups
    public interface IUsergroupsService36
    {
        IEnumerable<Usergroup> GetAll36();
        IEnumerable<Usergroup> GetById36(int id);
        Task<IResult> Create36(Usergroup input);
        Task<IResult> Update36(int id, Usergroup input);
        Task<IResult> Delete36(int id);
    }

    // 37 Userhelp
    public interface IUserhelpService37
    {
        IEnumerable<Userhelp> GetAll37();
        IEnumerable<Userhelp> GetById37(int id);
        Task<IResult> Create37(Userhelp input);
        Task<IResult> Update37(int id, Userhelp input);
        Task<IResult> Delete37(int id);
    }

    // 38 Userprofile
    public interface IUserprofileService38
    {
        IEnumerable<Userprofile> GetAll38();
        IEnumerable<Userprofile> GetById38(int id);
        Task<IResult> Create38(Userprofile input);
        Task<IResult> Update38(int id, Userprofile input);
        Task<IResult> Delete38(int id);
    }

    // 39 Usersession
    public interface IUsersessionService39
    {
        IEnumerable<Usersession> GetAll39();
        IEnumerable<Usersession> GetById39(int id);
        Task<IResult> Create39(Usersession input);
        Task<IResult> Update39(int id, Usersession input);
        Task<IResult> Delete39(int id);
    }

    // 40 V2Userprofile
    public interface IUserprofileService40
    {
        IEnumerable<Userprofile> GetAll40();
        IEnumerable<Userprofile> GetById40(int id);
        Task<IResult> Create40(Userprofile input);
        Task<IResult> Update40(int id, Userprofile input);
        Task<IResult> Delete40(int id);
    }

    // 41 WeatherForecast
    public interface IWeatherForecastService41
    {
        IEnumerable<WeatherForecast> GetForecast41();
    }
}*/
}