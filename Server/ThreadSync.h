#ifndef __THREAD_LOCK_H__
#define __THREAD_LOCK_H__

class CritSect
{
public:
    friend class ThreadLock;
    CritSect() { InitializeCriticalSection(&_critSection); }
    ~CritSect() { DeleteCriticalSection(&_critSection); }
private:
    void Acquire(){EnterCriticalSection(&_critSection);}
    void Release(){LeaveCriticalSection(&_critSection);}

    CRITICAL_SECTION _critSection;
};

class ThreadLock
{
public:
     ThreadLock(CritSect& critSect):_critSect(critSect) { _critSect.Acquire(); }
     ~ThreadLock(){_critSect.Release();}
private:
    CritSect& _critSect;
};
#endif
