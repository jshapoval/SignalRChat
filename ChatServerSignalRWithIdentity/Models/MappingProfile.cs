﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatServerSignalRWithIdentity.Data.DTO;

namespace ChatServerSignalRWithIdentity.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Message, MessageResponse>();
            CreateMap<MessageResponse, Message>();

            CreateMap<AppUser, AppUserResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Login, opt => opt.MapFrom(x => x.UserName))
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(x => x.Avatar.Square_100Id));

            CreateMap<AppUserResponse, AppUser>();
            //CreateMap<Dialog, DialogDTO>();
        }
    }
}
