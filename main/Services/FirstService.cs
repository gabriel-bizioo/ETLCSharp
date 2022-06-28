using System;
using System.Linq;

namespace main.Services;

class FirstService{
    public void NewTable(){

        using(var context = new analytic_dataContext())
        {
            var Query = context.Diagnosticos.Join(context.Pacientes, dg => dg.IdPaciente, pc => pc.Id, (dg, pc) => new
            {
                IdPaciente = pc.Id,
                IdClasse = pc.IdClasseSocial,
                IdDoenca = dg.IdDoenca,
                Idade = pc.Idade
            }).Join(context.ClasseSocials, dgpc => dgpc.IdClasse, cs => cs.Id, (dgpc, cs) => new
            {
                Teto = cs.SalarioTeto,
                IdDoenca = dgpc.IdDoenca,
                Idade = dgpc.Idade
            }).Join(context.Doencas, dgpccs => dgpccs.IdDoenca, dn => dn.Id, (dgpccs, dn) => new
            {
                NomeDoenca = dn.Nome,
                Salario = dgpccs.Teto,
                Idade = dgpccs.Idade
            }).GroupBy(x => x.NomeDoenca)
            .Select(g => new {
                salariomedio = g.Average(x => x.Salario),
                idademedia = g.Average(i => i.Idade),
                Doenca = g.Key
            });

            using(var context2 = new ets_dadosContext())
            {
                var table = context2.NewTables;

                foreach(var line in Query){
                    var Line  = new NewTable()
                    {
                        Doenca = line.Doenca,
                        MediaSalarial = (int)line.salariomedio,
                        MediaIdade = (int)line.idademedia
                    };
                    
                    table.Add(Line);
                }
            }
        }


    }

    public void DoencaIdadeRegiao()
    {
        using(var context = new analytic_dataContext())
        {
            var Query = context.Diagnosticos.Join(context.Pacientes, dg => dg.IdPaciente, pc => pc.Id, (pc, dg) => new
            {

            });
        }
    }
}