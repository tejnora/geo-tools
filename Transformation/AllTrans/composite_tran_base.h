#ifndef composite_tran_base_h_
#define composite_tran_base_h_

namespace all_tran{
using namespace std;
using namespace GNU_gama;

//---------------------------------------------------------------------------

class composite_tran_base : public all_tran_base  
{
public:
composite_tran_base( const Mat<>& XXi,const Mat<>& xxi) : 
		all_tran_base( XXi, xxi, 0 )  
		{}        

composite_tran_base( const Mat<>& XXi,const Mat<>& xxi, const Vec<> v ) : 
		all_tran_base( XXi, xxi, 0 ), iv(v)  
		{}		 

// vyresit 
//zatim nelze implicitni, hotove klice ani reset
/*
composite_tran_base(const Vec<> hh, const Vec<> v)
composite_tran_base(const Vec<> hh)
composite_tran_base()
void reset(const Mat<>& XXi,const Mat<>& xxi) 
void reset(const Vec<> hh)
*/

virtual ~composite_tran_base (){}

all_tran::all_tran_base* p1;
all_tran::all_tran_base* p2;

Vec<> iv,v1,v2;

virtual bool constants_from_first_transformation(Vec<> hah,Vec<>& v2){ return false; }

virtual void solve()    
{	  
	if(!is_solved)
	{		
		//constants
		int c1 = p1->get_c();
		if( c1 != 0 )
		{
			v1.reset(c1);
			for(int i = 1; i<= c1; i++)	v1(i) = iv(i);
		}		
		//constants
		
		//init
		int pp1 = p1->get_p();
		int pp2 = p2->get_p();
		p = pp1 + pp2;
		h.reset(p);
		q = p1->get_q() + p2->get_q();
		//init

		Vec<> hah;
		Mat<> X1;

		//p1		
		if( v1.dim()==0 )	p1->reset(*Xi,*xi);
		else	p1->reset(*Xi,*xi,v1);	
		p1->solve();
		hah = p1->get_solution();
		for(int i = 1; i<= pp1; i++)	h(i) = hah(i);
		//p1

		X1 = p1->transform_points(*xi);	
		
		//constants
		int c2 = p2->get_c();
		bool from_t1;
		if( c2 != 0 )	from_t1 = constants_from_first_transformation(hah,v2);
		if( (c2 != 0) && !from_t1 )
		{
			v2.reset(c2);
			for(int i = 1; i<= c2; i++)	v2(i) = iv(c1+i);
		}		
		//constants

		//p2		
		if( v2.dim() == 0 )	p2->reset(*Xi,X1);
		else	p2->reset(*Xi,X1,v2);
		p2->solve();
		hah = p2->get_solution();
		for(int i = 1; i<= pp2; i++)	h(pp1+i) = hah(i);
		//p2

		//beacouse of thin_plate_spline_2d
		//tps cant be the first transformation in composition
		if(pp2 != p2->get_p())
		{
			int ppp2 = p2->get_p();
			p = pp1 + ppp2;
			Vec<> help = h;
			h.reset(p);
			for(int i = 1; i <=p; i++) h(i) = help(i);
		}
		//beacouse of thin_plate_spline_2d

		//sig0 and v
		v.reset(n*r1+q);		
		Mat<> X2 = p2->transform_points(X1);							
		sig0 = 0;		
		for(int i = 1; i <= r1; i++)
			for(int j=1 ; j <= n ; j++)
			{				
				v( (i-1)*n + j ) = X2(i,j)-(*Xi)(i,j);
				sig0 += ( v( (i-1)*n + j )*v( (i-1)*n + j ) );				
			}        
		if((r1*n+q-p)>0) sig0 = sqrt(sig0/(r1+(double)(q-p)/n));
		else    sig0 = sqrt(sig0/r1);

		nofc = p1->nofc + p2->nofc;
		//sig0
		is_solved=true;   		
	}
	else throw at_exception("composite_tran_base::solve","Transformation key is already solved");	
}

ostream& report_constants(ostream& out) const                           //virtual
	{			
		out<<'\n'<<tt;
		out<<'\n'<<tt<<" is the composition of transformations: ";
		out<<p1->get_tt()<<" and "<<p2->get_tt()<<'\n';
		p1->report(out);
		out<<"and"<<'\n';
		p2->report(out);
		out<<"------- Composite report --------"<<endl;
        return out;
	}


void ApproxSolution(){}
void fill_matrixes() {}

Mat<> transform_points(const Mat<>& x)
{
	if(is_solved)
	{
		Mat<> X1 = p1->transform_points(x);
		Mat<> X2 = p2->transform_points(X1);		
		return X2;
	}	
	else
	{
		throw at_exception("composite_tran_base::transform_points","Transformation key is not solved yet. Call method \"solve()\" first");
	}
}
			
};

}   // namespace spat_fig
#endif