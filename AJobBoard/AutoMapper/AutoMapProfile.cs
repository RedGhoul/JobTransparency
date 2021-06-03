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
            CreateMap<JobPosting, JobPostingDTO>()
                .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(
                        x => x.Tags))
                .ForMember(dest => dest.KeyPhrases,
                    opt => opt.MapFrom(
                        x => x.KeyPhrases));
            
            CreateMap<JobPostingDTO, JobPosting>();

            CreateMap<KeyPhraseDTO, KeyPhrase>();
            CreateMap<KeyPhrase, KeyPhraseDTO>();

            CreateMap<SentimentDTO, Sentiment>();
            CreateMap<Sentiment, SentimentDTO>();

        }
    }
}
