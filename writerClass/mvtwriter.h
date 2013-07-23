#include <fstream>
#include <string>

namespace MVT
{
	class MVTWriter
	{
		std::fstream	file;
		std::string		fname;

		int curFrame;

		void setFname(std::string);

		public:
			MVTWriter(int);
			MVTWriter(int, std::string);

			void beginFrame();
			void record(int, int, double);

			~MVTWriter();

	};
}

