#define debug

using NLog;

namespace Jqs
{
	public class BasicLogger
	{
	    public static Logger logger = 
			LogManager.LoadConfiguration(
#if debug
				"C:/Users/aiyo/Desktop/AcqCnc/config/NLog.config"
#else
                "config/NLog.config"
#endif
            ).GetLogger("jqs");
	}
}