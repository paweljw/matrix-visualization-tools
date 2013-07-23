#include "mvtwriter.h"

#include <ctime>

namespace MVT
{
	void MVTWriter::setFname(std::string s)
	{
		this->fname = s;
	}

	MVTWriter::MVTWriter(int size)
	{
		char buffer[100];
		sprintf(buffer, "recording_%d.mvt", time(NULL));

		setFname(std::string(buffer));

		file.open(fname, std::ios_base::trunc);

		file << size << std::endl;

		curFrame = 0;
	}

	MVTWriter::MVTWriter(int size, std::string name)
	{
		setFname(name);
		file.open(fname, std::ios_base::trunc);
		file << size << std::endl;
		curFrame = 0;
	}

	void MVTWriter::beginFrame()
	{
		curFrame++;
		file << "---FRAME::" << curFrame << "---" << std::endl;
	}

	void MVTWriter::record(int x, int y, double v)
	{
		file << x << "," << y << "|" << v << std::endl;
	}

	MVTWriter::~MVTWriter()
	{
		file.close();
	}
}