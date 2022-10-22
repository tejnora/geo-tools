#ifndef all_tran_dlt_plus_rd2_h_
#define all_tran_dlt_plus_rd2_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------
class dlt_plus_rd2 : public composite_tran_base  
{
public:
dlt_plus_rd2(const Mat<>& XXi,const Mat<>& xxi) : composite_tran_base(XXi, xxi)
        {
			tt="dlt_plus_rd2";
			p1 = &in_dlt;
			p2 = &in_rd2;
		}
/*
dlt_plus_rd2(const Vec<> hh) : composite_tran_base(hh)
{	
	tt="dlt_plus_rd2";
	p1 = &in_dlt;
	p2 = &in_rd2;
}
*/

virtual ~dlt_plus_rd2 () {}

bool constants_from_first_transformation(Vec<> hah,Vec<>& v2)
{
	Mat<> a(3,1),b(3,1),c(3,1);		
	for(int i = 1; i<=3; i++)	
	{
		a(i,1) = hah(i);
		b(i,1) = hah(4+i);
		c(i,1) = hah(8+i);
	}
	double x0,y0;
	x0 = (trans(a)*c)(1,1)/(trans(c)*c)(1,1);
	y0 = (trans(b)*c)(1,1)/(trans(c)*c)(1,1);
	v2.reset(2);
	v2(1) = x0; v2(2) = y0;	
	return true;
}

protected:
all_tran::dlt in_dlt;
all_tran::rd2 in_rd2;			
};

}   // namespace spat_fig
#endif

