select * from classe_social
select * from estados
select * from pacientes
select * from doencas
select * from diagnosticos
select * from regioes


/* m�dia sal�rial - qtd pacientes - estado */
select COUNT(pacientes.id) as 'qtd. pacientes', floor(avg(salario_teto - salario_piso)) as m�dia, estados.nome as estado from pacientes
inner join estados on estados.id = pacientes.id_estado
inner join classe_social on classe_social.id = pacientes.id_classe_social
group by estados.nome order by m�dia

/* ocorr�ncias de doen�as em cada m�s por regi�o*/
select count(id_doenca) as 'qtd. casos', doencas.nome, DATEPART(MONTH, data_diagnostico) as M�s, regioes.nome_regiao as Regi�o from diagnosticos 
inner join doencas on diagnosticos.id_doenca = doencas.id
inner join pacientes on pacientes.id = diagnosticos.id_paciente
inner join estados on estados.id = pacientes.id_estado
inner join regioes on regioes.id = estados.id_regiao
group by DATEPART(MONTH, data_diagnostico), doencas.nome, regioes.nome_regiao
order by [M�s], Regi�o

/* m�dia de idade e m�dia salarial de cada doen�a */
select avg(pacientes.idade) as 'm�dia idade', floor(avg(salario_teto - salario_piso)) as 'm�dia sal�rial' , doencas.nome from diagnosticos
inner join pacientes on pacientes.id = diagnosticos.id_paciente
inner join doencas on doencas.id = diagnosticos.id_doenca
inner join classe_social on classe_social.id = pacientes.id_classe_social
group by doencas.nome order by [m�dia sal�rial]

/* ocorr�ncias de doen�as em estado */
select count(id_doenca) as 'qtd. casos', doencas.nome as doen�a, estados.nome as estado from diagnosticos 
inner join doencas on diagnosticos.id_doenca = doencas.id
inner join pacientes on diagnosticos.id_paciente = pacientes.id
inner join estados on estados.id = pacientes.id_estado
group by estados.nome, doencas.nome
order by estados.nome







/* ocorr�ncias de doen�as em estado */
select count(id_doenca) as 'qtd. casos', doencas.nome as doen�a, estados.nome as estado from diagnosticos
inner join doencas on diagnosticos.id_doenca = doencas.id
inner join pacientes on diagnosticos.id_paciente = pacientes.id
inner join estados on estados.id = pacientes.id_estado
group by estados.nome, doencas.nome
order by estados.nome

with teste(qtd_casos, doen�a, estado, estado_id) as 
(
	SELECT count(id_doenca) over (partition by department order by id) as qtd_casos, doencas.nome as doen�a, estados.nome as estado, 1 as estado_id from diagnosticos
	inner join doencas on diagnosticos.id_doenca = doencas.id
	inner join pacientes on diagnosticos.id_paciente = pacientes.id
	inner join estados on estados.id = pacientes.id_estado
	where estados.id = 1
	UNION ALL
	SELECT count(id_doenca) over (partition by department order by id) as qtd_casos, doencas.nome as doen�a, estados.nome as estado, estado_id + 1 from teste
	inner join estados on estados.id = estado_id
	inner join pacientes on estados.id = pacientes.id_estado
	inner join diagnosticos on diagnosticos.id_paciente = pacientes.id
	inner join doencas on doencas.id = diagnosticos.id_doenca
	where estados.id = estado_id
)

SELECT * FROM teste OPTION(MAXRECURSION 26)


/* where data_diagnostico between '2021-01-01' and '2021-02-01' */
