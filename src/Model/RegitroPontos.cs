namespace fun_pontoeletronico.Model;

public class RegitroPontos
{
    public int Id { get; set; }

    public string Email { get; set; }
    
    public DateTime Registro { get; set; }
    
    public bool MudancaAutorizada { get; set; } = false;
    public string Tipo { get; set; }
}
