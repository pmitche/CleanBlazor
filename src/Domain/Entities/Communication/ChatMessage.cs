﻿using BlazorHero.CleanArchitecture.Domain.Primitives;

namespace BlazorHero.CleanArchitecture.Domain.Entities.Communication;

public class ChatMessage<TUser> : AggregateRoot<long> where TUser : IChatUser
{
    public string FromUserId { get; set; }
    public string ToUserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedDate { get; set; }
    public virtual TUser FromUser { get; set; }
    public virtual TUser ToUser { get; set; }
}
