namespace Fcg.Auth.Common
{
    public class Response : IResponse
    {
        public List<string> Erros { get; private set; } = [];
        public bool HasErrors => Erros.Count > 0;

        public Response AddError(string erro)
        {
            if (string.IsNullOrWhiteSpace(erro))
                return this;

            if (!Erros.Contains(erro))
                Erros.Add(erro);

            return this;
        }

        public Response Append(IResponse response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            foreach (var erro in response.Erros)
                AddError(erro);

            return this;
        }

        public override string ToString() => string.Join("; ", Erros);
    }
}
