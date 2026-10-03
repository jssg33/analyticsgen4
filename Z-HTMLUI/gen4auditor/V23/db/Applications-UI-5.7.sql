-- CockyAuditor build 5.7: application UI codebase, vendors and operating systems.
-- UI_Codebase is the key field (mobile, html, ionic or react); it decides which of the vendor / OS columns apply:
--   html  -> UI_HTMLVendor, OS_UIHTML
--   react -> UI_REACTVendor, OS_UIREACT
--   ionic, mobile -> either set
--   every app -> APIVendor, OS_API
alter table Applications add  UI_Codebase varchar(255);
alter table Applications add  APIVendor varchar(255);
alter table Applications add  UI_HTMLVendor varchar(255);
alter table Applications add  UI_REACTVendor varchar(255);
alter table Applications add  OS_UIHTML varchar(255);
alter table Applications add  OS_UIREACT varchar(255);
alter table Applications add  OS_API varchar(255);

-- Matching properties on Enterprise.Models.Application (nullable strings):
--   public string? UI_Codebase { get; set; }
--   public string? APIVendor { get; set; }
--   public string? UI_HTMLVendor { get; set; }
--   public string? UI_REACTVendor { get; set; }
--   public string? OS_UIHTML { get; set; }
--   public string? OS_UIREACT { get; set; }
--   public string? OS_API { get; set; }
-- With the default JSON settings these come back as uI_Codebase, apiVendor, uI_HTMLVendor, uI_REACTVendor,
-- oS_UIHTML, oS_UIREACT and oS_API. CockyAuditor ignores case and underscores, so other spellings work too.
