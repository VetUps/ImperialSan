using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ImperialSanWPF.Utils
{
    static class ResponseErrorHandler
    {
        static public async Task<string> ProcessErrors(HttpResponseMessage response, List<string> errorOrder = null)
        {
            try
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

                if (errorResponse.Errors == null)
                    return $"Ошибка с кодом: {errorResponse.Status}";

                if (errorOrder == null)
                    errorOrder = errorResponse.Errors.Keys.ToList();

                if (errorResponse.Errors != null)
                {
                    List<string> errorMessages = new List<string>();

                    foreach (var error in errorOrder)
                    {
                        if (errorResponse.Errors.TryGetValue(error, out string[] values))
                            errorMessages.Add(values[0]);
                    }

                    return errorMessages[0];
                }

                return $"Неизвестная ошибка: {response.StatusCode} {errorResponse}";
            }
            catch (Exception ex)
            {
                return $"Ошибка обработки: {ex.Message}";
            }
        }
    }
}
