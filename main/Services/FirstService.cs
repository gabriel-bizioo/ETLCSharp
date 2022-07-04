using System;
using System.Linq;
namespace main.Services;

public class FirstService{
    public static void NewTable(){

        using(var context = new analytic_dataContext())
        {
            var Query = context.Diagnosticos
            .Join(context.Pacientes, dg => dg.IdPaciente, pc => pc.Id, (dg, pc) => new
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
            .Select(g => new 
            {
                salariomedio = (int)g.Average(x => x.Teto),
                idademedia = g.Average(i => i.Idade),
                Doenca = g.Key
            })
            .OrderBy(c => c.salariomedio);

            using(var context2 = new ets_dadosContext())
            {
                var table = context2.NewTables;

                foreach(var line in Query)
                {
                    var Line  = new NewTable()
                    {
                        Doenca = line.Doenca,
                        MediaSalarial = (int)line.salariomedio,
                        MediaIdade = (int)line.idademedia
                    };
                    
                    table.Add(Line);
                }
                context2.SaveChanges();
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
                IdadeMedia = Math.Floor(q.Average(x => x.Idade)),
                Regiao = q.Key.Regiao 
            })
            .OrderBy(q => q.Doenca)
            .ThenBy(q => q.Regiao);

            using(var context2 = new ets_dadosContext())
            {
                var table = context2.DoençaIdadeRegiaos;

                foreach(var line in Query)
                {
                    var Line  = new DoençaIdadeRegiao()
                    {
                        Doenca = line.Doenca,
                        MediaIdade = (int)line.IdadeMedia,
                        Regiao = line.Regiao
                    };
                    
                    table.Add(Line);
                }
                context2.SaveChanges();
            }
        }
    }

    public static void DiagnosticosClasseSocial()
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
                ClasseSocial = pc.ClasseSocial,
                Diagnostico = pc.Diagnostico
            })
            .GroupBy(cs => cs.ClasseSocial)
            .Select(q => new
            {
                ClasseSocial = q.Key,
                QtdDiagnosticos = q.Count(cs => cs.Diagnostico > -1)
            });

            using(var context2 = new ets_dadosContext())
            {
                var table = context2.DiagnosticosPorClasses;

                foreach(var line in Query){
                    var Line  = new DiagnosticosPorClasse()
                    {
                        QuantidadeDiagnosticos = line.QtdDiagnosticos,
                        ClasseSocial = line.ClasseSocial
                    };
                    
                    table.Add(Line);
                }
                context2.SaveChanges();
            }
        }
    }

    public static void DiagnosticosClasseSocialMes()
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
                    ClasseSocial = pc.ClasseSocial,
                    Diagnostico = pc.Diagnostico
                })
                .GroupBy(g => new 
                {
                    g.Mes,
                    g.ClasseSocial
                })
                .Select(q => new
                {
                    ClasseSocial = q.Key.ClasseSocial,
                    Mes = q.Key.Mes,
                    QtdDiagnosticos = q.Count(cs => cs.Diagnostico > -1)
                })
                .OrderBy(q => q.ClasseSocial)
                .ThenBy(q => q.Mes);

            using(var context2 = new ets_dadosContext())
            {
                var table = context2.DiagnosticosClasseMes;

                foreach(var line in Query){
                    Console.WriteLine(line.ClasseSocial.ToString());
                    var Line  = new DiagnosticosClasseMe()
                    {
                        
                        QuantidadeDiagnosticos = line.QtdDiagnosticos,
                        ClasseSocial = line.ClasseSocial.ToString(),
                        Mes = line.Mes
                    };
                    
                    table.Add(Line);
                    context2.SaveChanges();
                }
            }
        }
    }

    public static void  ReincidenciaMesRegiao()
    {
        var query = new object();

        using(var context = new analytic_dataContext()){

            var Query  = context.Diagnosticos
                .Join(context.Doencas, b => b.IdDoenca, d => d.Id, (b,d)=>new
                {
                IdDoenca = d.Id,
                IdPaciente = b.IdPaciente,
                Mes = b.DataDiagnostico,
                NomeDoenca = d.Nome
                })
                .Join(context.Pacientes, b=>b.IdPaciente, d=> d.Id, (b,d) =>new 
                {
                    IdEstado = d.IdEstado,
                    IdPaciente = b.IdPaciente,
                    IdDoenca = b.IdDoenca,
                    NomeDoenca = b.NomeDoenca,
                    Mes = b.Mes
                })
                .Join(context.Estados, b => b.IdEstado, d => d.Id, (b,d) => new
                {
                    IdPaciente = b.IdPaciente,
                    IdDoenca = b.IdDoenca,
                    NomeDoenca = b.NomeDoenca,
                    Mes = b.Mes,
                    IdRegiao = d.IdRegiao
                })
                .Join(context.Regioes, b => b.IdRegiao, d => d.Id, (b,d) => new
                {
                    IdPaciente = b.IdPaciente,
                    IdDoenca = b.IdDoenca,
                    NomeDoenca = b.NomeDoenca,
                    Mes = b.Mes,
                    IdRegiao = d.Id,
                    NomeRegiao = d.NomeRegiao
                })
                .GroupBy(x => new
                {
                    Mes = x.Mes,
                    Regiao = x.NomeRegiao,
                    NomeDoenca = x.NomeDoenca
                })
                .Select( n => new 
                {
                    Mes = n.Key.Mes,
                    NomeDoenca = n.Key.NomeDoenca,
                    NomeRegiao = n.Key.Regiao,
                    Reicidencia = n.Select(x=> x.IdDoenca).Count()
                })
                .Where(q => q.Reicidencia > 1);
            
            using(var context2 = new ets_dadosContext())
            {
                var table = context2.ReiciendenciaMesesRegia;

                foreach(var line in Query){
                    var Line  = new ReiciendenciaMesesRegium()
                    {
                        Regiao = line.NomeRegiao,
                        NomeDoenca = line.NomeDoenca,
                        Mes = (int)line.Mes.Month,
                        Reicidencia = line.Reicidencia
                    };
                    
                    table.Add(Line);
                    context2.SaveChanges();
                }
            }
        }
    }

     public static void PacienteClasseEstado()
    {
        using(var context = new analytic_dataContext())
        {
            var Query = context.Pacientes
                .Join(context.ClasseSocials, pc => pc.IdClasseSocial, cs => cs.Id, (pc, cs) => new
                {
                    Paciente = pc.Id,
                    EstadoPaciente = pc.IdEstado,
                    ClasseSocial = cs.Id,
                })
                .Join(context.Estados, cs => cs.EstadoPaciente, es => es.Id, (cs, es) => new
                {
                    Paciente = cs.Paciente,
                    NomeEstado = es.Nome,
                    RegiaoPaciente = es.IdRegiao,
                    ClasseSocial = cs.ClasseSocial
                })
                .GroupBy(g => new
                { 
                    g.ClasseSocial,
                    g.NomeEstado
                })
                .Select(q => new
                {
                    ClasseSocial = q.Key.ClasseSocial,
                    Estado = q.Key.NomeEstado,
                    QtdPacientes = q.Count(es => es.Paciente > -1)                    
                });

            using(var context2 = new ets_dadosContext())
            {
                var table = context2.PacientesClasseEstados;

                foreach(var line in Query)
                {
                    var Line  = new PacientesClasseEstado()
                    {
                        QuantidadePacientes = line.QtdPacientes,
                        ClasseSocial = line.ClasseSocial.ToString(),
                        Estado = line.Estado
                    };
                    
                    table.Add(Line);
                    context2.SaveChanges();
                }
            }
        }
    }

    public static void OcorrenciasClasseSocialRegiao()
    {
        using(var context = new analytic_dataContext())
        {
            var Query = context.Diagnosticos
                .Join(context.Pacientes, dg => dg.IdPaciente, pc => pc.Id, (dg, pc) => new
                {
                    ClasseSocial = pc.IdClasseSocial,
                    Estado = pc.IdEstado,
                    Diagnostico = dg.Id,
                    IdDoenca = dg.IdDoenca
                })
                .Join(context.ClasseSocials, pc => pc.ClasseSocial, cs => cs.Id, (pc, cs) => new
                {
                    ClasseSocial = cs.Id,
                    Estado = pc.Estado,
                    Diagnostico = pc.Diagnostico,
                    IdDoenca = pc.IdDoenca
                })
                .Join(context.Estados, cs => cs.Estado, es => es.Id, (cs, es) => new
                {
                    ClasseSocial = cs.ClasseSocial,
                    Regiao = es.IdRegiao,
                    Diagnostico = cs.Diagnostico,
                    IdDoenca = cs.IdDoenca
                })
                .Join(context.Regioes, es => es.Regiao, rg => rg.Id, (es, rg) => new
                {
                    ClasseSocial = es.ClasseSocial,
                    Regiao = rg.NomeRegiao,
                    Diagnostico = es.Diagnostico,
                    IdDoenca = es.IdDoenca
                })
                .Join(context.Doencas, rg => rg.IdDoenca, dc => dc.Id, (rg, dc) => new
                {
                    ClasseSocial = rg.ClasseSocial,
                    Regiao = rg.Regiao,
                    Diagnostico = rg.Diagnostico,
                    Doenca = dc.Nome
                })
                .GroupBy(g => new
                {
                    g.Doenca,
                    g.ClasseSocial,
                    g.Regiao
                })
                .Select(q => new
                {
                    QtdOcorrencias = q.Count(dc => dc.Diagnostico > -1),
                    Doenca = q.Key.Doenca,
                    ClasseSocial = q.Key.ClasseSocial,
                    Regiao = q.Key.Regiao
                });
                
            using(var context2 = new ets_dadosContext())
            {
                var table = context2.OcorrenciasClasseSocialRegiaos;
                foreach(var line in Query){
                    var Line  = new OcorrenciasClasseSocialRegiao()
                    {
                        QuantidadeOcorrencias = line.QtdOcorrencias,
                        NomeDoença = line.Doenca,
                        ClasseSocial = line.ClasseSocial.ToString(),
                        Regiao = line.Regiao
                    };
                        
                    table.Add(Line);
                }
                context2.SaveChanges();
            }
        }
    }

    public static void IncidenciasPorIdade()
    {
        using(var context = new analytic_dataContext())
        {
            var Query = context.Diagnosticos
                .Join(context.Pacientes, dg => dg.IdPaciente, pc => pc.Id, (dg, pc) => new
                {
                    Estado = pc.IdEstado,
                    QtdOcorrencias = dg.Id,
                    Doenca = dg.IdDoenca,
                    Idade = pc.Idade
                })
                .Join(context.Estados, pc => pc.Estado, es => es.Id, (pc, es) => new
                {
                    Estado = es.Nome,
                    QtdOcorrencias = pc.QtdOcorrencias,
                    Doenca = pc.Doenca,
                    Idade = pc.Idade
                })
                .Join(context.Doencas, es => es.Doenca, dc => dc.Id, (es, dc) => new
                {
                    Estado = es.Estado,
                    QtdOcorrencias = es.QtdOcorrencias,
                    Doenca = dc.Nome,
                    Idade = es.Idade
                })
                .GroupBy(g => new
                {
                    g.Doenca,
                    g.Estado
                })
                .Select(q => new
                {
                    Doenca = q.Key.Doenca,
                    QtdOcorrencias = q.Count(dc => dc.QtdOcorrencias > -1),
                    Idade = q.Average(dc => dc.Idade),
                    Estado = q.Key.Estado
                });
            
            using(var context2 = new ets_dadosContext())
            {
                var table = context2.IncidenciasPorIdades;
                foreach(var line in Query)
                {
                    var Line  = new IncidenciasPorIdade()
                    {
                         QuantidadeOcorrencias = line.QtdOcorrencias,
                         NomeDoenca = line.Doenca,
                        Estados = line.Estado,
                        Idade = (int)line.Idade
                    };
                    table.Add(Line);
                }
                context2.SaveChanges();
            }
        }
    }
}