#pragma once

namespace libCoin3D {
public ref class IRaypickCallback
{
	public:
		IRaypickCallback();
		virtual void clicked() = 0;
		virtual void clicked(int x, int y) = 0;
};
}