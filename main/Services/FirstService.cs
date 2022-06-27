using System;
using System.Linq;

namespace main.Services;

class FirstService{
    public void NewTable(){
        
        int MediaSalarial;

        string Doenca;

        int MediaIdade;

        using(var context = new analytic_dataContext())
        {
            var Media = context.Pacientes.Join(context.ClasseSocials, pc => pc.IdClasseSocial, cs => cs.Id, (pc, cs) => new
            {
                salario = cs.SalarioTeto,
            });       

        }
    }
}