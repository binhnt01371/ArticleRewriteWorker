using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticleRewriteWorker.Dtos
{
	public class SpinRewiterResponseDto
	{
		public string status { get; set; }
		public string response { get; set; }
		public int api_requests_made { get; set; }
		public int api_requests_available { get; set; }
		public string protected_terms { get; set; }
		public string nested_spintax { get; set; }
		public string confidence_level { get; set; }
	}
}
