using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
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
