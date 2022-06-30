using System;
using System.Linq;
namespace main.Services;

public class FirstService{
    public static void NewTable(){

        using(var context = new analytic_dataContext())
        {
            var Query = context.Diagnosticos.Join(context.Pacientes, dg => dg.IdPaciente, pc => pc.Id, (dg, pc) => new
            {
                IdPaciente = pc.Id,
                IdClasse = pc.IdClasseSocial,
                IdDoenca = dg.IdDoenca,
                Idade = pc.Idade
            })
            .Join(context.ClasseSocials, dgpc => dgpc.IdClasse, cs => cs.Id, (dgpc, cs) => new
            {
                Teto = cs.SalarioTeto,
                IdDoenca = dgpc.IdDoenca,
                Idade = dgpc.Idade
            })
            .Join(context.Doencas, dgpccs => dgpccs.IdDoenca, dn => dn.Id, (dgpccs, dn) => new
            {
                NomeDoenca = dn.Nome,
                Teto = dgpccs.Teto,
                Idade = dgpccs.Idade
            })
            .GroupBy(x => x.NomeDoenca)
            .Select(g => new {
                salariomedio = (int)g.Average(x => x.Teto),
                idademedia = g.Average(i => i.Idade),
                Doenca = g.Key
            }).OrderBy(c => c.salariomedio);

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

    public static void DoencaIdadeRegiao()
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
            .GroupBy(dc => new 
            {
                dc.Doenca,
                dc.Regiao
            })
            .Select(q => new
            {
                Doenca = q.Key.Doenca,
                IdadeMedia = q.Average(x => x.Idade),
                Regiao = q.Key.Regiao
            });

            foreach(var line in Query)
            {
                Console.WriteLine(line);
            }
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

    public void DiagnosticosClasseSocialMes()
    {
        using(var context = new analytic_dataContext())
        {
            var Query = context.Diagnosticos
            .Join(context.Pacientes, dg => dg.IdPaciente, pc => pc.Id, (dg, pc) => new
            {
                Paciente = pc.Id,
                Diagnostico = dg.Id,
                Mes = dg.DataDiagnostico.Month,
                ClasseSocial = pc.IdClasseSocial
            })
            .Join(context.ClasseSocials, pc => pc.ClasseSocial, cs => cs.Id, (pc, cs) => new
            {
                Mes = pc.Mes,
                ClasseSocial = cs.SalarioTeto,
                Diagnostico = pc.Diagnostico
            })
            .GroupBy(cs => cs.ClasseSocial)
            .Select(q => new
            {
                Mes =  q.All(cs => cs.Mes > 0),
                ClasseSocial = q.Key,
                QtdDiagnosticos = q.Count(cs => cs.Diagnostico > -1)
            });
        }
    }

    public void  ReincidenciaMesRegiao()
    {
        using(var context = new analytic_dataContext())
        {
            var Query = context.Diagnosticos
            .Join(context.Doencas, dg => dg.IdDoenca, dc => dc.Id, (dg, dc) => new
            {
                IdDiagnostico = dg.Id,
                Paciente = dg.IdPaciente,
                Doenca = dc.Nome,
                Mes = dg.DataDiagnostico.Month,

            })
            .Join(context.Pacientes, dc => dc.Paciente, pc => pc.Id, (dc, pc) => new
            {
                IdDiagnosticos = dc.IdDiagnostico,
                Estado = pc.IdEstado,
                Doenca = dc.Doenca,
                Mes = dc.Mes
            })
            .Join(context.Estados, pc => pc.Estado, es => es.Id, (pc, es) => new 
            {
                IdDiagnostico = pc.IdDiagnosticos,
                Regiao = es.IdRegiao,
                Doenca = pc.Doenca,
                Mes = pc.Mes
            })
            .Join(context.Regioes, es => es.Regiao, rg => rg.Id, (es, rg) => new
            {
                Doenca = es.Doenca,
                Regiao = rg.NomeRegiao,
                Mes = es.Mes
            })
            .GroupBy(g => g.Doenca)
            .Select( q => new
            {
                Mes = q.Key
            });
        }
    }

    public void PacienteClasseEstado()
    {
        using(var context = new analytic_dataContext())
        {
            
        }
    }
}