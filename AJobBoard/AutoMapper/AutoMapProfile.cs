using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Models.DTO;
using AutoMapper;

namespace Jobtransparency.AutoMapper
{
    public class AutoMapProfile : Profile
    {
        public AutoMapProfile()
        {
            CreateMap<JobPosting, JobPostingDTO>();
            CreateMap<JobPostingDTO, JobPosting>();

            CreateMap<KeyPhraseDTO, KeyPhrase>();
            CreateMap<KeyPhrase, KeyPhraseDTO>();


        }
    }
}
