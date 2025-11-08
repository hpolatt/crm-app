import { TablePagination } from '@mui/material'

interface PaginationProps {
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages?: number
  hasPreviousPage?: boolean
  hasNextPage?: boolean
  onPageChange: (page: number) => void
  onPageSizeChange: (size: number) => void
}

export default function Pagination({
  totalCount,
  pageNumber,
  pageSize,
  totalPages: _totalPages,
  hasPreviousPage: _hasPreviousPage,
  hasNextPage: _hasNextPage,
  onPageChange,
  onPageSizeChange,
}: PaginationProps) {
  const handleChangePage = (_event: unknown, newPage: number) => {
    onPageChange(newPage + 1) // MUI uses 0-based, backend uses 1-based
  }

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    onPageSizeChange(parseInt(event.target.value, 10))
    onPageChange(1)
  }

  return (
    <TablePagination
      component="div"
      count={totalCount}
      page={pageNumber - 1} // Convert 1-based to 0-based for MUI
      onPageChange={handleChangePage}
      rowsPerPage={pageSize}
      onRowsPerPageChange={handleChangeRowsPerPage}
      rowsPerPageOptions={[5, 10, 25, 50]}
    />
  )
}
