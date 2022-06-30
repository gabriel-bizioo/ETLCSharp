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
            })
            .GroupBy(x => x.NomeDoenca)
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
            var Query = context.Diagnosticos
            .Join(context.Pacientes, dg => dg.IdPaciente, pc => pc.Id, (dg, pc) => new
            {
                IdDoenca = dg.IdDoenca,
                IdPaciente = pc.Id,
                IdEstado = pc.IdEstado,
                Idade = pc.Idade
            })
            .Join(context.Estados, pc => pc.IdEstado, es => es.Id, (pc, es) => new
            {
                IdRegiao = es.IdRegiao,
                Regiao = es.Nome,
                IdDoenca = pc.IdDoenca,
                Idade = pc.Idade
            })
            .Join(context.Doencas, es => es.IdDoenca, dc => dc.Id, (es, dc) => new
            {
                Idade = es.Idade,
                Regiao = es.Regiao,
                Doenca = dc.Nome
            })
            .GroupBy(dc => dc.Doenca)
            .Select(q => new
            {
                Doenca = q.Key,
                IdadeMedia = q.Average(x => x.Idade)
            });
        }
    }

    public void DiagnosticosClasseSocial()
    {
        using(var context = new analytic_dataContext())
        {
            var Query = context.Diagnosticos
            .Join(context.Pacientes, dg => dg.IdPaciente, pc => pc.Id, (dg, pc) => new
            {
                Paciente = pc.Id,
                Diagnostico = dg.Id,
                ClasseSocial = pc.IdClasseSocial
            })
            .Join(context.ClasseSocials, pc => pc.ClasseSocial, cs => cs.Id, (pc, cs) => new
            {
                ClasseSocial = cs.SalarioTeto,
                Diagnostico = pc.Diagnostico
            })
            .GroupBy(cs => cs.ClasseSocial)
            .Select(q => new
            {
                ClasseSocial = q.Key,
                QtdDiagnosticos = q.Count(cs => cs.Diagnostico > -1)
            });
        }
    }
}