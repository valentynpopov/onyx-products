namespace Onyx.Products.API.Models;

public record Product(
    long ProductId,
    string SKU,
    string Name,
    string Colour,
    string Description);
