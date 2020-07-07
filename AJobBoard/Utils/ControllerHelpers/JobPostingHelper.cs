using AJobBoard.Models;
using AJobBoard.Models.View;

namespace AJobBoard.Utils.ControllerHelpers
{
    public static class JobPostingHelper
    {

        public static void SetupViewBag(HomeIndexViewModel homeIndexVM, dynamic ViewBag)
        {
            ViewBag.Location = homeIndexVM.FindModel.Location;
            ViewBag.KeyWords = homeIndexVM.FindModel.KeyWords;
            ViewBag.Date = homeIndexVM.FindModel.Date;
            ViewBag.MaxResults = homeIndexVM.FindModel.MaxResults;
            ViewBag.TotalJobs = homeIndexVM.FindModel.MaxResults != 0 ? homeIndexVM.FindModel.MaxResults : 100;
            ViewBag.Page = homeIndexVM.FindModel.Page;
        }

        public static HomeIndexViewModel SetDefaultFindModel(HomeIndexViewModel homeIndexVM)
        {
            if (homeIndexVM == null)
            {
                homeIndexVM = new HomeIndexViewModel
                {
                    FindModel = new FindModel
                    {
                        Location = "anywhere",
                        KeyWords = "",
                        MaxResults = 100,
                        Page = 1
                    }
                };
            }
            else if (homeIndexVM.FindModel == null)
            {
                homeIndexVM.FindModel = new FindModel();

                homeIndexVM = FillFindModel(homeIndexVM);
            }
            else
            {
                homeIndexVM = FillFindModel(homeIndexVM);
            }
            return homeIndexVM;
        }

        private static HomeIndexViewModel FillFindModel(HomeIndexViewModel homeIndexVM)
        {
            homeIndexVM.FindModel.KeyWords ??= "";
            homeIndexVM.FindModel.Location ??= "";
            if (homeIndexVM.FindModel.MaxResults == 0)
            {
                homeIndexVM.FindModel.MaxResults = 100;
            }

            if (homeIndexVM.FindModel.Page == 0)
            {
                homeIndexVM.FindModel.Page = 1;
            }

            return homeIndexVM;
        }
    }
}
