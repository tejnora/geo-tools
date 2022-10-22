#ifndef all_tran_projective_x0y0_plus_something_h_
#define all_tran_projective_x0y0_plus_something_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class projective_x0y0_plus_rd : public composite_tran_base 
{
public:								
projective_x0y0_plus_rd(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )  // v(1) == x0 and v(2) == y0
	{
		tt="projective_x0y0_plus_rd";
		p1 = &in_projective_x0y0;
		p2 = &in_rd;
	}        
/*
projective_x0y0_plus_rd(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="projective_x0y0_plus_rd";
		p1 = &in_projective_x0y0;
		p2 = &in_rd;
	} 
*/
virtual ~projective_x0y0_plus_rd ()	{}

bool constants_from_first_transformation(Vec<> hah,Vec<>& v2)
{	
	v2.reset(2);
	v2(1) = hah(13); v2(2) = hah(14);
	return true;
}

protected:
	all_tran::projective_x0y0 in_projective_x0y0;
	all_tran::rd in_rd;
};


class projective_x0y0_plus_rd_td : public composite_tran_base 
{
public:								
projective_x0y0_plus_rd_td(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )  // v(1) == x0 and v(2) == y0
	{
		tt="projective_x0y0_plus_rd_td";
		p1 = &in_projective_x0y0;
		p2 = &in_rd_td;
	}        
/*
projective_x0y0_plus_rd(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="projective_x0y0_plus_rd_td";
		p1 = &in_projective_x0y0;
		p2 = &in_rd_td;
	} 
*/
virtual ~projective_x0y0_plus_rd_td ()	{}

bool constants_from_first_transformation(Vec<> hah,Vec<>& v2)
{	
	v2.reset(2);
	v2(1) = hah(13); v2(2) = hah(14);
	return true;
}

protected:
	all_tran::projective_x0y0 in_projective_x0y0;
	all_tran::rd_td in_rd_td;
};


class projective_x0y0_plus_rd2 : public composite_tran_base 
{
public:								
projective_x0y0_plus_rd2(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )  // v(1) == x0 and v(2) == y0
	{
		tt="projective_x0y0_plus_rd2";
		p1 = &in_projective_x0y0;
		p2 = &in_rd2;
	}        
/*
projective_x0y0_plus_rd2(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="projective_x0y0_plus_rd2";
		p1 = &in_projective_x0y0;
		p2 = &in_rd2;
	} 
*/
virtual ~projective_x0y0_plus_rd2 ()	{}

bool constants_from_first_transformation(Vec<> hah,Vec<>& v2)
{	
	v2.reset(2);
	v2(1) = hah(13); v2(2) = hah(14);
	return true;
}

protected:
	all_tran::projective_x0y0 in_projective_x0y0;
	all_tran::rd2 in_rd2;
};


class projective_x0y0_plus_quadratic_2d : public composite_tran_base 
{
public:								
projective_x0y0_plus_quadratic_2d(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )
	{
		tt="projective_x0y0_plus_quadratic_2d";
		p1 = &in_projective_x0y0;
		p2 = &in_quadratic_2d;
	}        
/*
projective_x0y0_plus_quadratic_2d(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="projective_x0y0_plus_quadratic_2d";
		p1 = &in_projective_x0y0;
		p2 = &in_quadratic_2d;
	} 
*/

virtual ~projective_x0y0_plus_quadratic_2d ()	{}

protected:
	all_tran::projective_x0y0 in_projective_x0y0;
	all_tran::quadratic_2d in_quadratic_2d;
};


class projective_x0y0_plus_cubic_2d : public composite_tran_base 
{
public:								
projective_x0y0_plus_cubic_2d(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )
	{
		tt="projective_x0y0_plus_cubic_2d";
		p1 = &in_projective_x0y0;
		p2 = &in_cubic_2d;
	}        
/*
projective_x0y0_plus_cubic_2d(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="projective_x0y0_plus_cubic_2d";
		p1 = &in_projective_x0y0;
		p2 = &in_cubic_2d;
	} 
*/

virtual ~projective_x0y0_plus_cubic_2d ()	{}

protected:
	all_tran::projective_x0y0 in_projective_x0y0;
	all_tran::cubic_2d in_cubic_2d;
};

class projective_plus_cubic_2d : public composite_tran_base 
{
public:								
projective_plus_cubic_2d(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )
	{
		tt="projective_plus_cubic_2d";
		p1 = &in_projective;
		p2 = &in_cubic_2d;
	}        
/*
projective_plus_cubic_2d(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="projective_plus_cubic_2d";
		p1 = &in_projective;
		p2 = &in_cubic_2d;
	} 
*/

virtual ~projective_plus_cubic_2d ()	{}

protected:
	all_tran::projective in_projective;
	all_tran::cubic_2d in_cubic_2d;
};


class projective_x0y0_plus_quartic_2d : public composite_tran_base 
{
public:								
projective_x0y0_plus_quartic_2d(const Mat<>& XXi,const Mat<>& xxi,const Vec<> v) : composite_tran_base( XXi,xxi,v )
	{
		tt="projective_x0y0_plus_quartic_2d";
		p1 = &in_projective_x0y0;
		p2 = &in_quartic_2d;
	}        
/*
projective_x0y0_plus_quartic_2d(const Vec<> hh) : composite_tran_base(hh)
	{
		tt="projective_x0y0_plus_quartic_2d";
		p1 = &in_projective_x0y0;
		p2 = &in_quartic_2d;
	} 
*/

virtual ~projective_x0y0_plus_quartic_2d ()	{}

protected:
	all_tran::projective_x0y0 in_projective_x0y0;
	all_tran::quartic_2d in_quartic_2d;
};



}   // namespace all_tran
#endif