#include "BTService.h"

	int BTService::Discover(DeviceDetails* out)
	{
		DeviceDetails* dd = new DeviceDetails();
		
		std::cout << "discovering\n";
		DataGenerator();
		out = dd;
		return 1;
	}
	int BTService::Connect(DeviceDetails)
	{
		std::cout << "attempting to connect to device\n";
		return 0;
	}

	int BTService::ReceiveData()
	{
		return 0;
	}

	int BTService::ProcessData(NormalData* out)
	{
		/*
			Data format: uses 16 bit data chunks
				--Header
				0x0008 (Protocol for identifying type)
				0x0003 (Length of data)

				--Data
				0xA064
				0x74B4
				etc...

				--Footer
				0xFFFF


		*/
		bool Active = true;
		short Length = -1;
		short Protocol = -1;
		short databuf[4] = {0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF};
		NormalData ret;
		while (Active)
		{
			if (Data.size() != 0)
			{
				unsigned short chunk = Data.front(); Data.pop();

				if (expected == Expectedtype::Protocol)
				{
					if (chunk < 32)
					{
						//normal
						Protocol = chunk;
						if (chunk < 12)
						{
							ret.finger = new Axis();
							ret.finger->nAxisNum = chunk - 2;
						}
						else if (chunk < 32)
						{
							//other undefined
						}
						expected = Expectedtype::Length;
					}
					//out of range
				}
				else if (expected == Expectedtype::Length)
				{
					Length = chunk;
					expected = Expectedtype::Data;
				}
				else if (expected == Expectedtype::Data)
				{
					if (chunk != (unsigned short)(0xFFFF)) //not a footer
					{
						if (Protocol < 7)
						{
							//data is of double type
							for (int p = 0; p < 4; p++)
							{
								if (databuf[p] == (short)0xFFFF)
								{
									//free space
									databuf[p] = chunk;
									break;
								}
							}

						}
					}
					else
					{
						//do final things and return

						expected = Expectedtype::Protocol;

						if (Protocol < 12)
						{
							double* value = new double(0);
							for (int p = 0; p < 4; p++) //convert data buffer to double
							{
								if (databuf[p] != 0xFFFF)
								{
									*((short*)value + p) = databuf[p];
								}
								else break;
							}
							ret.finger->val = *value;
							if (out != nullptr)
								*out = ret;
							return 0;
						}
					}
				}

			}
			else
			{
				return 0;
			}
		}

		return 0;
	}

	void BTService::DataGenerator()
	{
		for (int k = 0; k < 1; k++)
		{
			//int n = (rand() * 36 + 4) % 40;
			int n = 7;

			//Header
			Data.push(0x0001);
			Data.push(n - 3);
			//Data
			double val = partdata;
			for (int i = 0; i < n - 3; i++)
			{
				Data.push(*((short*)&val + i));
				std::cout << *((short*)&val + i) << '\n';
			}
			std::cout << '\n';
			//Footer
			Data.push(0xFFFF);

			NormalData* returnad = new NormalData();
			ProcessData(returnad);
			std::cout << returnad->finger->val << '\n';
		}
	}
