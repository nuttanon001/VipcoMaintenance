using AutoMapper;
using VipcoMaintenance.Models.Machines;
using VipcoMaintenance.Models.Maintenances;
using VipcoMaintenance.ViewModels;


namespace VipcoMaintenance.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Item

            // Item
            CreateMap<Item, ItemViewModel>()
                .ForMember(x => x.ItemStatusString,
                            o => o.MapFrom(s => System.Enum.GetName(typeof(ItemStatus), s.ItemStatus)))
                .ForMember(x => x.BranchString,
                            o => o.MapFrom(s => s.Branch == null ? "ไม่ระบุ" : s.Branch.Name))
                .ForMember(x => x.Branch,o => o.Ignore())
                .ForMember(x => x.ItemImage,o => o.Ignore())
                // ItemType
                .ForMember(x => x.ItemTypeString,
                           o => o.MapFrom(s => s.ItemType == null ? "ไม่ระบุ" : s.ItemType.Name))
                .ForMember(x => x.ItemType,o => o.Ignore());
            CreateMap<ItemViewModel, Item>();

            #endregion Item

            #region RequireMaintenace
            // RequireMaintenance
            CreateMap<RequireMaintenance, RequireMaintenanceViewModel>()
                .ForMember(x => x.RequireStatusString,
                            o => o.MapFrom(s => System.Enum.GetName(typeof(RequireStatus), s.RequireStatus)))
                .ForMember(x => x.BranchString,
                            o => o.MapFrom(s => s.Branch != null ? s.Branch.Name : "ไม่ระบุ"))
                .ForMember(x => x.Branch, o => o.Ignore())
                .ForMember(x => x.ItemCode,
                            o => o.MapFrom(s => s.Item != null ? $"{s.Item.ItemCode}/{s.Item.Name}" : "ไม่ระบุ"))
                .ForMember(x => x.Item, o => o.Ignore());
            CreateMap<RequireMaintenanceViewModel, RequireMaintenance>();
            #endregion

            #region ItemMaintenance
            // ItemMaintenance
            CreateMap<ItemMaintenance, ItemMaintenanceViewModel>()
                .ForMember(x => x.StatusMaintenanceString,
                             o => o.MapFrom(s => System.Enum.GetName(typeof(StatusMaintenance), s.StatusMaintenance)))
                .ForMember(x => x.TypeMaintenanceString,
                            o => o.MapFrom(s => s.TypeMaintenance == null ? "ไม่ระบุ" : s.TypeMaintenance.Name))
                .ForMember(x => x.TypeMaintenance, o => o.Ignore())
                .ForMember(x => x.WorkGroupMaintenanceString,
                            o => o.MapFrom(s => s.WorkGroupMaintenance == null ? "ไม่ระบุ" : s.WorkGroupMaintenance.Name))
                .ForMember(x => x.WorkGroupMaintenance,o => o.Ignore())
                .ForMember(x => x.ItemCode,
                            o => o.MapFrom(s => s.RequireMaintenance == null ? "ไม่ระบุ" : $"{s.RequireMaintenance.Item.ItemCode}/{s.RequireMaintenance.Item.Name}"))
                .ForMember(x => x.RequireMaintenance, o => o.Ignore());

            #endregion

            #region SparePart
            //SparePart
            CreateMap<SparePart, SparePartViewModel>()
                .ForMember(x => x.RequisitionStockSps, o => o.Ignore())
                .ForMember(x => x.WorkGroup, o => o.Ignore());
            #endregion

            #region ReceiveStockSp
            CreateMap<ReceiveStockSp, ReceiveStockSpViewModel>()
                .ForMember(x => x.SparePartName,
                            o => o.MapFrom(s => s.SparePart == null ? "ไม่ระบุ" : s.SparePart.Name))
                .ForMember(x => x.SparePart,o => o.Ignore())
                .ForMember(x => x.MovementStockSp, o => o.Ignore());
            #endregion

            #region AdjustStockSp
            CreateMap<AdjustStockSp, AdjustStockSpViewModel>()
                .ForMember(x => x.SparePartName,
                            o => o.MapFrom(s => s.SparePart == null ? "ไม่ระบุ" : s.SparePart.Name))
                .ForMember(x => x.SparePart, o => o.Ignore())
                .ForMember(x => x.MovementStockSp, o => o.Ignore());
            #endregion

            #region RequisitionStockSp
            CreateMap<RequisitionStockSp,RequisitionStockSpViewModel>()
                .ForMember(x => x.SparePartName,
                        o => o.MapFrom(s => s.SparePart == null ? "ไม่ระบุ" : s.SparePart.Name))
                .ForMember(x => x.UnitPrice,
                        o => o.MapFrom(s => s.SparePart == null ? 0 : s.SparePart.UnitPrice))
                .ForMember(x => x.SparePart, o => o.Ignore())
                .ForMember(x => x.MovementStockSp, o => o.Ignore());
            #endregion

            #region ItemMainHasEmployee

            CreateMap<ItemMainHasEmployee, ItemMainHasEmployeeViewModel>();

            #endregion

            #region User

            //User
            CreateMap<User, UserViewModel>()
                // CuttingPlanNo
                .ForMember(x => x.NameThai,
                           o => o.MapFrom(s => s.EmpCodeNavigation == null ? "-" : $"คุณ{s.EmpCodeNavigation.NameThai}"))
                .ForMember(x => x.EmpCodeNavigation, o => o.Ignore());

            #endregion User
        }
    }
}
