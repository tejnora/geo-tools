#ifndef all_tran_dlt_2d_rd2_plus_tps_2d_h_
#define all_tran_dlt_2d_rd2_plus_tps_2d_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class dlt_2d_rd2_plus_tps_2d : public composite_tran_base  
{
public:

dlt_2d_rd2_plus_tps_2d(const Mat<>& XXi,const Mat<>& xxi, const Vec<> v) : composite_tran_base(XXi,xxi,v)
	//because of dependent number of variables in TPS
	, in_tps_2d( 2*(2*xxi.rows()+3) )
	{
		tt="dlt_2d_rd2_plus_tps_2d";
		p1 = &in_dlt_2d_rd2;
		p2 = &in_tps_2d;
	}

/*
dlt_2d_rd2_plus_tps_2d(const Vec<> hh) : composite_tran_base(hh)
	//because of dependent number of variables in TPS
	, in_tps_2d( hh.dim() - in_dlt_2d_rd2.get_p() )
	{
		tt="dlt_2d_rd2_plus_tps_2d";
		p1 = &in_dlt_2d_rd2;
		p2 = &in_tps_2d;
	}
*/
virtual ~dlt_2d_rd2_plus_tps_2d ()	{}

protected:
	all_tran::dlt_2d_rd2 in_dlt_2d_rd2;
	all_tran::thin_plate_spline_2d in_tps_2d;

};

}   // namespace all_tran
#endif