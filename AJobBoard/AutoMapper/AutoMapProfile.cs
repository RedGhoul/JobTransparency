using AJobBoard.Models.Dto;
using AJobBoard.Models.Entity;
using AutoMapper;
using Jobtransparency.Models.DTO;
using Jobtransparency.Models.Entity;

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

            CreateMap<SentimentDTO, Sentiment>();
            CreateMap<Sentiment, SentimentDTO>();

        }
    }
}
