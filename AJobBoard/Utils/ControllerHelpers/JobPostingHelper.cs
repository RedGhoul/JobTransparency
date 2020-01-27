using AJobBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AJobBoard.Utils.ControllerHelpers
{
    public class JobPostingHelper
    {
        public static List<JobPosting> ConfigurePaging(HomeIndexViewModel homeIndexVM, List<JobPosting> Jobs, int totalJobs, int MaxPage, int Page)
        {
            int PageSize = 12;

            var count = Jobs.Count();
            if (count > 25)
            {
                totalJobs = count;
                Jobs = Jobs.Skip((int)homeIndexVM.FindModel.Page * PageSize).Take(PageSize).ToList();
                if (PageSize == 0)
                {
                    MaxPage = 10;
                }
                else
                {
                    MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);
                }
                Page = homeIndexVM.FindModel.Page;
            }


            return Jobs;
        }



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

        public static HomeIndexViewModel FillFindModel(HomeIndexViewModel homeIndexVM)
        {
            homeIndexVM.FindModel.KeyWords = homeIndexVM.FindModel.KeyWords ?? "";
            homeIndexVM.FindModel.Location = homeIndexVM.FindModel.Location ?? "";
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
