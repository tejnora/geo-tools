/*  
    C++ Matrix/Vector templates (GNU Gama / matvec 0.9.25)
    Copyright (C) 1999  Ales Cepek <cepek@gnu.org>

    This file is part of the GNU Gama C++ Matrix/Vector template library.
    
    This library is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/

/*
 *  $Id: sortvec.h,v 1.1 2006/04/09 16:12:01 cepek Exp $
 *  http://www.gnu.org/software/gama/
 */

#ifndef GNU_gama_gMatVec_Sort_Vec__h_
#define GNU_gama_gMatVec_Sort_Vec__h_

#include <algorithm>
#include <matvec/vecbase.h>


namespace GNU_gama {

template <typename Float, typename Exc>
inline void sort(Vec<Float, Exc>& v)
  {
    typename Vec<Float, Exc>::iterator b = v.begin();
    typename Vec<Float, Exc>::iterator e = v.end();
    std::sort(b, e);
  }


}   // namespace GNU_gama

#endif



