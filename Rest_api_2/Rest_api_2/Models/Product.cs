﻿namespace Rest_api_2.Models;

public class Product
{
    public int? IdProduct { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
}