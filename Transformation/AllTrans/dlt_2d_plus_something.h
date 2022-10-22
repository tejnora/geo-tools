#ifndef all_tran_dlt_2d_plus_something_h_
#define all_tran_dlt_2d_plus_something_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class dlt_2d_plus_rd : public composite_tran_base 
{
public:								
dlt_2d_plus_rd(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )  // v(1) == x0 and v(2) == y0
	{
		tt="dlt_2d_plus_rd";
		p1 = &in_dlt_2d;
		p2 = &in_rd;
	}        
/*
dlt_2d_plus_rd(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="dlt_2d_plus_rd";
		p1 = &in_dlt_2d;
		p2 = &in_rd;
	} 
*/
virtual ~dlt_2d_plus_rd ()	{}

protected:
	all_tran::dlt_2d in_dlt_2d;
	all_tran::rd in_rd;
};


class dlt_2d_plus_rd_td : public composite_tran_base 
{
public:								
dlt_2d_plus_rd_td(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )  // v(1) == x0 and v(2) == y0
	{
		tt="dlt_2d_plus_rd_td";
		p1 = &in_dlt_2d;
		p2 = &in_rd_td;
	}        
/*
dlt_2d_plus_rd(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="dlt_2d_plus_rd_td";
		p1 = &in_dlt_2d;
		p2 = &in_rd_td;
	} 
*/
virtual ~dlt_2d_plus_rd_td ()	{}

protected:
	all_tran::dlt_2d in_dlt_2d;
	all_tran::rd_td in_rd_td;
};


class dlt_2d_plus_rd2 : public composite_tran_base 
{
public:								
dlt_2d_plus_rd2(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )  // v(1) == x0 and v(2) == y0
	{
		tt="dlt_2d_plus_rd2";
		p1 = &in_dlt_2d;
		p2 = &in_rd2;
	}        
/*
dlt_2d_plus_rd2(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="dlt_2d_plus_rd2";
		p1 = &in_dlt_2d;
		p2 = &in_rd2;
	} 
*/
virtual ~dlt_2d_plus_rd2 ()	{}

protected:
	all_tran::dlt_2d in_dlt_2d;
	all_tran::rd2 in_rd2;
};


class dlt_2d_plus_quadratic_2d : public composite_tran_base 
{
public:								
dlt_2d_plus_quadratic_2d(const Mat<>& XXi,const Mat<>& xxi) : composite_tran_base( XXi,xxi)
	{
		tt="dlt_2d_plus_quadratic_2d";
		p1 = &in_dlt_2d;
		p2 = &in_quadratic_2d;
	}        
/*
dlt_2d_plus_quadratic_2d(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="dlt_2d_plus_quadratic_2d";
		p1 = &in_dlt_2d;
		p2 = &in_quadratic_2d;
	} 
*/

virtual ~dlt_2d_plus_quadratic_2d ()	{}

protected:
	all_tran::dlt_2d in_dlt_2d;
	all_tran::quadratic_2d in_quadratic_2d;
};


class dlt_2d_plus_cubic_2d : public composite_tran_base 
{
public:								
dlt_2d_plus_cubic_2d(const Mat<>& XXi,const Mat<>& xxi) : composite_tran_base( XXi,xxi)
	{
		tt="dlt_2d_plus_cubic_2d";
		p1 = &in_dlt_2d;
		p2 = &in_cubic_2d;
	}        
/*
dlt_2d_plus_cubic_2d(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="dlt_2d_plus_cubic_2d";
		p1 = &in_dlt_2d;
		p2 = &in_cubic_2d;
	} 
*/

virtual ~dlt_2d_plus_cubic_2d ()	{}

protected:
	all_tran::dlt_2d in_dlt_2d;
	all_tran::cubic_2d in_cubic_2d;
};


class dlt_2d_plus_quartic_2d : public composite_tran_base 
{
public:								
dlt_2d_plus_quartic_2d(const Mat<>& XXi,const Mat<>& xxi) : composite_tran_base( XXi,xxi)
	{
		tt="dlt_2d_plus_quartic_2d";
		p1 = &in_dlt_2d;
		p2 = &in_quartic_2d;
	}        
/*
dlt_2d_plus_quartic_2d(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="dlt_2d_plus_quartic_2d";
		p1 = &in_dlt_2d;
		p2 = &in_quartic_2d;
	} 
*/

virtual ~dlt_2d_plus_quartic_2d ()	{}

protected:
	all_tran::dlt_2d in_dlt_2d;
	all_tran::quartic_2d in_quartic_2d;
};

class dlt_2d_rd_td_plus_cubic_2d : public composite_tran_base 
{
public:								
dlt_2d_rd_td_plus_cubic_2d(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )  // v(1) == x0 and v(2) == y0
	{
		tt="dlt_2d_rd_td_plus_cubic_2d";
		p1 = &in_dlt_2d_rd_td;
		p2 = &in_cubic_2d;
	}        

virtual ~dlt_2d_rd_td_plus_cubic_2d ()	{}

protected:
	all_tran::dlt_2d_rd_td in_dlt_2d_rd_td;
	all_tran::cubic_2d in_cubic_2d;
};

}   // namespace all_tran
#endif