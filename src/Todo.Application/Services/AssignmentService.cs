﻿using AutoMapper;
using Todo.Domain.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Todo.Application.Contracts;
using Todo.Application.DTO.Assignment;
using Todo.Domain.Contracts.Repository;
using Todo.Application.Contracts.Services;

namespace Todo.Application.Services;

public class AssignmentService : BaseService, IAssignmentService
{
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IAssignmentListRepository _assignmentListRepository;

    public AssignmentService(
        IMapper mapper,
        INotificator notificator,
        IAssignmentRepository assignmentRepository,
        IAssignmentListRepository assignmentListRepository,
        IHttpContextAccessor httpContextAccessor) : base(notificator)
    {
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _assignmentRepository = assignmentRepository;
        _assignmentListRepository = assignmentListRepository;
    }

    public async Task<AssignmentDto?> GetById(Guid id)
    {
        var getAssignment = await _assignmentRepository.GetById(id, GetUserId());

        if (getAssignment == null)
        {   
            Notify("Não foi possível encontrar a tarefa correspondente.");
            return null;
        }

        return _mapper.Map<AssignmentDto>(getAssignment);
    }

    public async Task<AssignmentDto?> Create(AddAssignmentDto addAssignmentDto)
    {
        var getAssignment = await _assignmentListRepository.GetById(addAssignmentDto.AssignmentListId);

        if (getAssignment == null)
        {
            Notify("Não foi possível encontrar a lista de tarefas correspondente.");
            return null;
        }

        var assignment = _mapper.Map<Assignment>(addAssignmentDto);
        assignment.UserId = GetUserId();

        await _assignmentRepository.Create(assignment);

        return _mapper.Map<AssignmentDto>(assignment);
    }

    public async Task<AssignmentDto?> Update(Guid id, UpdateAssignmentDto updateAssignmentDto)
    {
        if (id != updateAssignmentDto.Id)
        {
            Notify("O id informado é inválido");
            return null;
        }

        var getAssignment = await _assignmentRepository.GetById(id, GetUserId());
        
        if (getAssignment == null)
        {
            Notify("Não foi possível encontrar a tarefa correspondente.");
            return null;
        }

        _mapper.Map(updateAssignmentDto, getAssignment);

        await _assignmentRepository.Update(getAssignment);

        return _mapper.Map<AssignmentDto>(updateAssignmentDto);
    }
    
    public async Task Delete(Guid id)
    {
        var getAssignment = await _assignmentRepository.GetById(id, GetUserId());

        if (getAssignment == null)
        {
            Notify("Não foi possível encontrar a tarefa correspondente.");
            return;
        }

        await _assignmentRepository.Delete(getAssignment);
    }

    private Guid GetUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userId == null ? Guid.Empty : Guid.Parse(userId);
    }
}