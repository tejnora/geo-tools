#ifndef all_tran_dlt_plus_tps_2d_h_
#define all_tran_dlt_plus_tps_2d_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------
class dlt_plus_tps_2d : public composite_tran_base  
{
public:
dlt_plus_tps_2d(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )
	//because of dependent number of variables in TPS
	, in_tps_2d( 2*(2*xxi.rows()+3) )
	{
			tt="dlt_plus_tps_2d";
			p1 = &in_dlt;
			p2 = &in_tps_2d;
	}
/*
dlt_plus_tps_2d(const Vec<> hh) : composite_tran_base(hh)
//because of dependent number of variables in TPS
	, in_tps_2d( hh.dim() - in_dlt.get_p() )
{
	tt="dlt_plus_tps_2d";
	p1 = &in_dlt;
	p2 = &in_tps_2d;
}
*/

virtual ~dlt_plus_tps_2d () {}

protected:
all_tran::dlt in_dlt;
all_tran::thin_plate_spline_2d in_tps_2d;			
};

}   // namespace spat_fig
#endif

