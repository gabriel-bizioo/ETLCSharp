select * from classe_social
select * from estados
select * from pacientes
select * from doencas
select * from diagnosticos
select * from regioes


/* média salárial - qtd pacientes - estado */
select COUNT(pacientes.id) as 'qtd. pacientes', floor(avg(salario_teto - salario_piso)) as média, estados.nome as estado from pacientes
inner join estados on estados.id = pacientes.id_estado
inner join classe_social on classe_social.id = pacientes.id_classe_social
group by estados.nome order by média

/* ocorrências de doenças em cada mês por região*/
select count(id_doenca) as 'qtd. casos', doencas.nome, DATEPART(MONTH, data_diagnostico) as Mês, regioes.nome_regiao as Região from diagnosticos 
inner join doencas on diagnosticos.id_doenca = doencas.id
inner join pacientes on pacientes.id = diagnosticos.id_paciente
inner join estados on estados.id = pacientes.id_estado
inner join regioes on regioes.id = estados.id_regiao
group by DATEPART(MONTH, data_diagnostico), doencas.nome, regioes.nome_regiao
order by [Mês], Região

/* média de idade e média salarial de cada doença */
select avg(pacientes.idade) as 'média idade', floor(avg(salario_teto - salario_piso)) as 'média salárial' , doencas.nome from diagnosticos
inner join pacientes on pacientes.id = diagnosticos.id_paciente
inner join doencas on doencas.id = diagnosticos.id_doenca
inner join classe_social on classe_social.id = pacientes.id_classe_social
group by doencas.nome order by [média salárial]

/* ocorrências de doenças em estado */
select count(id_doenca) as 'qtd. casos', doencas.nome as doença, estados.nome as estado from diagnosticos 
inner join doencas on diagnosticos.id_doenca = doencas.id
inner join pacientes on diagnosticos.id_paciente = pacientes.id
inner join estados on estados.id = pacientes.id_estado
group by estados.nome, doencas.nome
order by estados.nome







/* ocorrências de doenças em estado */
select count(id_doenca) as 'qtd. casos', doencas.nome as doença, estados.nome as estado from diagnosticos
inner join doencas on diagnosticos.id_doenca = doencas.id
inner join pacientes on diagnosticos.id_paciente = pacientes.id
inner join estados on estados.id = pacientes.id_estado
group by estados.nome, doencas.nome
order by estados.nome

with teste(qtd_casos, doença, estado, estado_id) as 
(
	SELECT count(id_doenca) over (partition by department order by id) as qtd_casos, doencas.nome as doença, estados.nome as estado, 1 as estado_id from diagnosticos
	inner join doencas on diagnosticos.id_doenca = doencas.id
	inner join pacientes on diagnosticos.id_paciente = pacientes.id
	inner join estados on estados.id = pacientes.id_estado
	where estados.id = 1
	UNION ALL
	SELECT count(id_doenca) over (partition by department order by id) as qtd_casos, doencas.nome as doença, estados.nome as estado, estado_id + 1 from teste
	inner join estados on estados.id = estado_id
	inner join pacientes on estados.id = pacientes.id_estado
	inner join diagnosticos on diagnosticos.id_paciente = pacientes.id
	inner join doencas on doencas.id = diagnosticos.id_doenca
	where estados.id = estado_id
)

SELECT * FROM teste OPTION(MAXRECURSION 26)


/* where data_diagnostico between '2021-01-01' and '2021-02-01' */
