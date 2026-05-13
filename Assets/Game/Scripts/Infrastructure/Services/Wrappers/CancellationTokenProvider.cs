using System.Threading;

namespace Infrastructure
{
	public sealed class CancellationTokenProvider
	{
		private CancellationTokenSource _cts;

		public void Init()
		{
			_cts = new CancellationTokenSource();
		}

		public void OnDispose()
		{
			_cts.Cancel();
			_cts.Dispose();
		}

		public CancellationTokenSource CreateLocalCts()
			=> CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
	}
}