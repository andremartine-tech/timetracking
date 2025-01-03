using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Specifications
{
    /// <summary>
    /// Cont�m especifica��es e valida��es para a entidade TimeLog.
    /// </summary>
    public static class TimeLogSpecifications
    {
        /// <summary>
        /// Verifica se dois registros de ponto possuem um intervalo v�lido.
        /// </summary>
        /// <param name="start">Registro de entrada.</param>
        /// <param name="end">Registro de sa�da.</param>
        /// <returns>True se o intervalo for v�lido; caso contr�rio, False.</returns>
        public static bool IsValidTimeLogInterval(TimeLog timelog)
        {
            return timelog.TimestampIn < timelog.TimestampOut;
        }

        /// <summary>
        /// Verifica se j� existe um registro de ponto duplicado para o mesmo hor�rio e tipo.
        /// </summary>
        /// <param name="logs">Lista de registros de ponto do usu�rio.</param>
        /// <param name="newLog">Novo registro de ponto a ser validado.</param>
        /// <returns>True se o registro for duplicado; caso contr�rio, False.</returns>
        public static bool IsDuplicateTimeLog(IEnumerable<TimeLog> logs, TimeLog newLog)
        {
            if (logs == null || newLog == null)
                throw new ArgumentNullException("Os registros e o novo registro n�o podem ser nulos.");

            return logs.Any(log =>
                log.TimestampIn == newLog.TimestampIn);
        }

        /// <summary>
        /// Valida se os registros de ponto do usu�rio est�o ordenados corretamente.
        /// </summary>
        /// <param name="logs">Lista de registros de ponto do usu�rio.</param>
        /// <returns>True se todos os registros estiverem ordenados; caso contr�rio, False.</returns>
        public static bool AreTimeLogsOrdered(IEnumerable<TimeLog> logs)
        {
            if (logs == null)
                throw new ArgumentNullException("Os registros de ponto n�o podem ser nulos.");

            var logList = logs.OrderBy(log => log.TimestampIn).ToList();
            for (int i = 0; i < logList.Count - 1; i++)
            {
                if (logList[i].TimestampIn >= logList[i + 1].TimestampIn)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica todos os registros de ponto associados a um usu�rio no m�s e ano de refer�ncia.
        /// </summary>
        /// <param name="start">Data e hora do registro de ponto de entrada.</param>
        /// <param name="end">Data e hora do registro de ponto de sa�da.</param>
        /// <param name="userId">Identificador do usu�rio.</param>
        /// <returns>Uma expression para ser usada na busca.</returns>
        public static Expression<Func<TimeLog, bool>> ByUserAndMonthYear(Guid userId, DateTime monthYear)
        {
            return log =>
                log.UserId == userId &&
                log.TimestampIn.Month == monthYear.Month &&
                log.TimestampIn.Year == monthYear.Year;
        }

        /// <summary>
        /// Verifica se existe sobreposi��o de registro de ponto.
        /// </summary>
        /// <param name="start">Data e hora do registro de ponto de entrada.</param>
        /// <param name="end">Data e hora do registro de ponto de sa�da.</param>
        /// <param name="userId">Identificador do usu�rio.</param>
        /// <returns>Uma expression para ser usada na busca.</returns>
        public static Expression<Func<TimeLog, bool>> HasOverlapWith(Guid id, DateTime start, DateTime end, Guid userId)
        {
            return log =>
                (id == Guid.Empty || log.Id != id) &&
                log.UserId == userId &&
                log.TimestampIn < end &&
                start < log.TimestampOut;
        }

    }
}
