﻿namespace WebApiAutores.Dtos;

public class EmailConfigurationDTO
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string FromName { get; set; }
    public string FromAddress { get; set; }
}