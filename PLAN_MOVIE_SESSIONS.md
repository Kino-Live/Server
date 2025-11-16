# План реализации: Показ доступных сеансов для фильма по датам

## Описание фичи
Пользователь выбирает фильм → видит доступные даты с сеансами → выбирает дату → видит конкретные сеансы на эту дату.

## Архитектурные особенности
- **Movie** (Фильм) - может иметь множество прокатов (MovieScreening)
- **MovieScreening** (Прокат) - один прокат на кинотеатр, связь Movie-Cinema
- **ShowTime** (Сеанс) - множество сеансов в рамках одного проката

## Структура реализации

### 1. DTO (Data Transfer Objects)

#### 1.1. `AvailableDatesDTO.cs`
```csharp
// Список доступных дат для фильма
public class AvailableDatesDTO
{
    public int MovieId { get; set; }
    public string MovieName { get; set; }
    public List<DateOnly> AvailableDates { get; set; } // Уникальные даты, где есть сеансы
}
```

#### 1.2. `ShowTimeByDateDTO.cs` (или расширить существующий ShowTimeDTO)
```csharp
// Сеанс с дополнительной информацией для отображения
public class ShowTimeByDateDTO
{
    public int ShowTimeId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ViewingFormat ViewingFormat { get; set; }
    public decimal TicketPrice { get; set; }
    public int HallId { get; set; }
    public string HallName { get; set; }
    public int CinemaId { get; set; }
    public string CinemaName { get; set; }
    public string CinemaAddress { get; set; }
    public ShowTimeStatus ShowTimeStatus { get; set; }
}
```

#### 1.3. `ShowTimesByDateResponseDTO.cs`
```csharp
// Ответ с сеансами на конкретную дату
public class ShowTimesByDateResponseDTO
{
    public int MovieId { get; set; }
    public string MovieName { get; set; }
    public DateOnly SelectedDate { get; set; }
    public List<ShowTimeByDateDTO> ShowTimes { get; set; }
}
```

### 2. Repository Layer (DAL)

#### 2.1. `IShowTimeRepository.cs` - добавить методы:
```csharp
// Получить уникальные даты, где есть активные сеансы для фильма
Task<List<DateOnly>> GetAvailableDatesByMovieIdAsync(int movieId);

// Получить сеансы для фильма на конкретную дату
Task<IEnumerable<ShowTime>> GetShowTimesByMovieIdAndDateAsync(int movieId, DateOnly date);
```

#### 2.2. `ShowTimeRepository.cs` - реализация:
- Метод `GetAvailableDatesByMovieIdAsync`:
  - Найти все MovieScreening для фильма с актуальным статусом
  - Найти все ShowTime для этих прокатов со статусом Active
  - Отфильтровать прошедшие даты (StartTime.Date >= DateOnly.FromDateTime(DateTime.Now))
  - Извлечь уникальные даты из StartTime
  - Отсортировать по дате

- Метод `GetShowTimesByMovieIdAndDateAsync`:
  - Найти все MovieScreening для фильма с актуальным статусом
  - Найти все ShowTime для этих прокатов со статусом Active
  - Отфильтровать по дате (StartTime.Date == date)
  - Отфильтровать прошедшие сеансы (StartTime >= DateTime.Now)
  - Включить связанные данные (Hall, Cinema, MovieScreening)
  - Отсортировать по времени начала

### 3. Service Layer (BLL)

#### 3.1. `IShowTimeService.cs` - добавить методы:
```csharp
// Получить доступные даты для фильма
Task<AvailableDatesDTO> GetAvailableDatesByMovieIdAsync(int movieId);

// Получить сеансы для фильма на конкретную дату
Task<ShowTimesByDateResponseDTO> GetShowTimesByMovieIdAndDateAsync(int movieId, DateOnly date);
```

#### 3.2. `ShowTimeService.cs` - реализация:
- Валидация существования фильма
- Вызов методов репозитория
- Маппинг в DTO
- Обработка ошибок

### 4. Controller Layer

#### 4.1. `ShowTimeController.cs` - добавить endpoints:
```csharp
// GET: api/ShowTime/available-dates/{movieId}
// Получить список доступных дат для фильма
[HttpGet("available-dates/{movieId}")]
public async Task<ActionResult<AvailableDatesDTO>> GetAvailableDatesByMovieIdAsync(int movieId)

// GET: api/ShowTime/by-movie/{movieId}/date/{date}
// Получить сеансы для фильма на конкретную дату
// Формат даты: 2024-01-15
[HttpGet("by-movie/{movieId}/date/{date}")]
public async Task<ActionResult<ShowTimesByDateResponseDTO>> GetShowTimesByMovieIdAndDateAsync(
    int movieId, 
    [FromRoute] DateOnly date)
```

### 5. Mapping Profiles

#### 5.1. `ShowTimeMappingProfile.cs` - добавить маппинги:
- `ShowTime` → `ShowTimeByDateDTO` (с включением Hall и Cinema)

### 6. Validation (опционально)

#### 6.1. Валидация параметров:
- Проверка существования фильма
- Проверка формата даты (автоматически через DateOnly в route)
- Проверка, что дата не в прошлом (обязательно - возвращать ошибку, если дата прошла)

### 7. Тестирование

#### 7.1. Unit тесты:
- Тесты репозитория
- Тесты сервиса
- Тесты контроллера

## Порядок реализации

1. ✅ **Создать DTO** - AvailableDatesDTO, ShowTimeByDateDTO, ShowTimesByDateResponseDTO
2. ✅ **Расширить Repository Interface** - добавить методы в IShowTimeRepository
3. ✅ **Реализовать Repository** - реализовать методы в ShowTimeRepository
4. ✅ **Расширить Service Interface** - добавить методы в IShowTimeService
5. ✅ **Реализовать Service** - реализовать методы в ShowTimeService
6. ✅ **Обновить Mapping Profile** - добавить маппинги для новых DTO
7. ✅ **Добавить Controller Endpoints** - добавить endpoints в ShowTimeController
8. ✅ **Тестирование** - протестировать функциональность

## Дополнительные соображения

### Фильтрация:
- Показывать только активные сеансы (ShowTimeStatus.Active)
- Учитывать актуальность проката (MovieScreeningRelevance)
- **Обязательно фильтровать прошедшие даты** - не возвращать даты, которые уже прошли
- Фильтровать сеансы, которые уже начались (StartTime < DateTime.Now)

### Производительность:
- Использовать AsNoTracking() для read-only операций
- Правильно использовать Include() для загрузки связанных данных (Hall, Cinema)
- Возможно, добавить индексы на MovieId, StartTime в БД
- Пагинация не требуется на данном этапе

### Бизнес-логика:
- **Группировка по кинотеатрам**: На текущем этапе система работает в рамках одного кинотеатра, но структура данных заложена с учетом расширения. В DTO включена информация о кинотеатре (CinemaId, CinemaName, CinemaAddress), что позволит легко добавить группировку на клиенте или в отдельном endpoint в будущем. Сейчас сеансы возвращаются простым списком, отсортированным по времени начала.
- Сортировка по времени начала
- Учет доступности зала (HallAvailability)
- **Фильтрация прошедших дат**: Автоматически исключать даты, которые уже прошли
- **Формат даты в URL**: `2024-01-15` (DateOnly)

